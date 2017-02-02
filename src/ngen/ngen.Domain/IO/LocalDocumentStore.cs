using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ngen.Core.IO;
using ngen.Core.Security;
using ngen.Data;
using ngen.Data.Model;
using ngen.Domain.EventArgs;
using ngen.Domain.Security;

namespace ngen.Domain.IO
{
    public class LocalDocumentStore : DocumentStoreBase, IDocumentStore
    {
        public LocalDocumentStore(Employee currentEmployee) : base(currentEmployee)
        {
        }

        public LocalDocumentStore(ngenDbContext dataContext, Employee currentEmployee)
            : base(dataContext, currentEmployee)
        {
        }

        public async Task OpenTempAsync(DocumentVersion version)
        {
            await SecurityManager.EnforceReadAccessAsync(version.DocumentId, CurrentEmployee);

            var document = await DataContext.Documents.FirstAsync(d => d.Id == version.DocumentId);

            CurrentFileName = document.FileName;

            var source = await GetPathToVersionFileAsync(version);
            var extension = Path.GetExtension(CurrentFileName);

            // generate unique filename
            string destination = $"{TempFileDirectory}\\1{extension}";
            var counter = 1;
            while (File.Exists(destination))
            {
                counter++;
                destination = $"{TempFileDirectory}\\{counter}.{extension}";
            }

            // decrypt the file then open it
            await DecryptAsync(source, destination);

            var process = Process.Start(destination);

            // fire completed event
            OnTransferProgress(new DocumentTransferEventArgs(CurrentFileName, DocumentTransferStatus.Complete, 100));

            // wait for the process to exit, then delete the temp file
            await process.WaitForExitAsync();

            try
            {
                File.Delete(destination);
            }
            catch
            {
            } // if it fails, file should be cleaned up next time PurgeTempFiles is called
        }

        public async Task AddAsync(string source, object entity, string changes = null)
        {
            // ensure the current employee can manage documents
            await SecurityManager.EnforceAsync(SystemPermission.ManageDocuments, CurrentEmployee);

            CurrentFileName = Path.GetFileName(source);

            // check if this is an existing document and therefore just needs a new version created
            if (await IsExistingDocumentAsync(entity))
                return;

            // create and save new Document and DocumentVersion entities to database
            var document = CreateDocumentForEntity(entity);
            DataContext.Documents.Add(document);
            await DataContext.SaveChangesAsync();

            try
            {
                await AddNewVersionToDocumentAsync(source, document, changes);
            }
            catch
            {
                // if something went wrong while creating new version, delete the new document record from the database
                DataContext.Documents.Remove(document);
                await DataContext.SaveChangesAsync();
                // now we can throw the exception
                throw;
            }

            // fire completed event
            OnTransferProgress(new DocumentTransferEventArgs(CurrentFileName, DocumentTransferStatus.Complete, 100));
        }

        public async Task DeleteAsync(Document document)
        {
            // ensure employee has permission to write to this document
            await SecurityManager.EnforceAsync(SystemPermission.ManageDocuments, CurrentEmployee);
            await SecurityManager.EnforceWriteAccessAsync(document.Id, CurrentEmployee);

            // get a list of all versions of this document
            var versions = await DataContext.DocumentVersions.Where(v => v.DocumentId == document.Id).ToListAsync();

            using (new NetworkShareConnection(SecureSettings.FileShareDirectory, SecureSettings.FileShareCredentials))
            {
                foreach (var version in versions)
                {
                    // delete the actual file
                    var source = await GetPathToVersionFileAsync(version);
                    if (File.Exists(source))
                        try
                        {
                            File.Delete(source);
                        }
                        catch
                        {
                            // TODO: if deleting version file failed, add as orphaned file to some sort of list of files to be deleted
                        }
                }
            }

            // remove all related records from the database
            await DeleteDocumentRecordsAsync(document);
        }

        public async Task CheckOutAsync(Document document)
        {
            // ensure employee has permission to write to this document
            await SecurityManager.EnforceAsync(SystemPermission.ManageDocuments, CurrentEmployee);
            await SecurityManager.EnforceWriteAccessAsync(document.Id, CurrentEmployee);

            var checkOutDirectory = SecureSettings.CheckOutDirectory;

            var destination = Path.Combine(checkOutDirectory, document.FileName);

            if (File.Exists(destination))
                throw new Exception($"A file named {document.FileName} already exists in your checkout directory.");

            var latestVersion = await GetLatestVersionAsync(document);

            var source = await GetPathToVersionFileAsync(latestVersion);

            await MarkAsCheckedOutAsync(document);

            await DecryptAsync(source, destination);

            // fire completed event
            OnTransferProgress(new DocumentTransferEventArgs(CurrentFileName, DocumentTransferStatus.Complete, 100));
        }

        public async Task CheckInAsync(Document document, string changes = null)
        {
            // ensure employee STILL has permission to write to this document
            await SecurityManager.EnforceAsync(SystemPermission.ManageDocuments, CurrentEmployee);
            await SecurityManager.EnforceWriteAccessAsync(document.Id, CurrentEmployee);

            var checkOutDirectory = SecureSettings.CheckOutDirectory;

            var pathToCheckOutFile = Path.Combine(checkOutDirectory, document.FileName);

            if (!File.Exists(pathToCheckOutFile))
            {
                await MarkAsCheckedInAsync(document);

                throw new FileNotFoundException(
                    $"Unable to locate checked out document with filename {document.FileName}");
            }

            // calculate hash value of the checked out file
            var newHash = await ComputeHashValueAsync(pathToCheckOutFile);

            var latestVersion = await GetLatestVersionAsync(document);

            // compare new hash to that of the latest version and if they're different, add new version
            if (newHash != latestVersion.Hash)
                await AddNewVersionToDocumentAsync(pathToCheckOutFile, document, changes);

            // delete the file from the checkout directory
            var deletedOk = true;
            try
            {
                File.Delete(pathToCheckOutFile);
            }
            catch
            {
                deletedOk = false;
            }

            // mark document as checked in
            if (deletedOk)
                await MarkAsCheckedInAsync(document);

            // fire completed event
            OnTransferProgress(new DocumentTransferEventArgs(CurrentFileName, DocumentTransferStatus.Complete, 100));
        }

        private async Task<string> GetEntityStoragePathAsync(object entity)
        {
            if (entity is DocumentFolder)
            {
                var folder = entity as DocumentFolder;

                return $"fid_{folder.Id}";
            }

            if (entity is Part)
            {
                var part = entity as Part;

                return $"cid_{part.CustomerId}\\pid_{part.Id}";
            }

            if (entity is PartVersion)
            {
                var version = entity as PartVersion;

                var part = DataContext.Parts.SingleOrDefault(p => p.Id == version.PartId);

                var partDir = await GetEntityStoragePathAsync(part);

                return $"{partDir}\\vid_{version.Id}";
            }

            throw new NotImplementedException(
                $"Attaching to an entity of the type {entity.GetType().Name} has not been implemented!");
        }

        private async Task<string> GetPathToVersionFileAsync(DocumentVersion version)
        {
            object entity = null;

            if (version.Document.Part != null)
                entity = version.Document.Part;
            else if (version.Document.PartVersion != null)
                entity = version.Document.PartVersion;
            else if (version.Document.DocumentFolder != null)
                entity = version.Document.DocumentFolder;

            var entityStoragePath = await GetEntityStoragePathAsync(entity);

            var localStoreDirectory = SecureSettings.FileShareDirectory;

            return Path.Combine(localStoreDirectory, entityStoragePath, version.Id.ToString());
        }

        private async Task AddNewVersionToDocumentAsync(string source, Document document, string changes)
        {
            // create and save the new DocumentVersion entity to the database
            var version = await CreateNewDocumentVersionAsync(source, document, changes);
            DataContext.DocumentVersions.Add(version);
            await DataContext.SaveChangesAsync();

            // generate the destination filename for the new version
            var destination = await GetPathToVersionFileAsync(version);

            try
            {
                // encrypt the source file to the destination file
                await EncryptAsync(source, destination);
            }
            catch
            {
                // if something went wrong during encryption, delete the new version record from the database
                DataContext.DocumentVersions.Remove(version);
                await DataContext.SaveChangesAsync();
                // now we can throw the exception
                throw;
            }

            // check if there are too many versions and delete the oldest one if there are
            var obsoleteVersion = await GetObsoleteVersionAsync(document);

            if (obsoleteVersion == null)
                return;

            var deletedOk = true;

            try
            {
                var fileToDelete = await GetPathToVersionFileAsync(obsoleteVersion);
                File.Delete(fileToDelete);
            }
            catch
            {
                deletedOk = false;
            }

            if (deletedOk)
            {
                DataContext.DocumentVersions.Remove(obsoleteVersion);
                await DataContext.SaveChangesAsync();
            }
        }
        
        private async Task EncryptAsync(string source, string destination)
        {
            var dir = Path.GetDirectoryName(destination);

            if (!Directory.Exists(dir))
            {
                using (
                    new NetworkShareConnection(SecureSettings.FileShareDirectory, SecureSettings.FileShareCredentials))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            
            var aes = new AESEncryptionProvider();
            aes.ProgressChanged += Encryption_ProgressChanged;

            using (new NetworkShareConnection(SecureSettings.FileShareDirectory, SecureSettings.FileShareCredentials))
            {
                await aes.EncryptFileAsync(source, destination, SecureSettings.EncryptionPassword);
            }
        }

        private async Task DecryptAsync(string source, string destination)
        {
            var aes = new AESEncryptionProvider();
            aes.ProgressChanged += Decryption_ProgressChanged;

            await aes.DecryptFileAsync(source, destination, SecureSettings.EncryptionPassword);
        }
        
        private void Decryption_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var args = new DocumentTransferEventArgs(CurrentFileName, DocumentTransferStatus.Decrypting,
                e.ProgressPercentage);

            OnTransferProgress(args);
        }

        private void Encryption_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var args = new DocumentTransferEventArgs(CurrentFileName, DocumentTransferStatus.Encrypting,
                e.ProgressPercentage);

            OnTransferProgress(args);
        }
    }
}