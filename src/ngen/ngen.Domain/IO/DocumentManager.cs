using System;
using System.Collections.Generic;
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
using ngen.Domain.Enum;
using ngen.Domain.EventArgs;
using ngen.Domain.Security;

namespace ngen.Domain.IO
{
    public class DocumentManager
    {
        private Dictionary<Process, string> _tempFilesToDelete = new Dictionary<Process, string>();
         
        /// <summary>
        /// The employee currently logged in
        /// </summary>
        private readonly Employee _employee;

        /// <summary>
        /// The name of the file currently being processed
        /// </summary>
        private string _currentFileName;

        private readonly ngenDbContext _db = new ngenDbContext();

        public DocumentManager(Employee employee)
        {
            _employee = employee;
        }

        public event EventHandler<DocumentTransferEventArgs> TransferProgress;

        public async Task OpenTempAsync(DocumentVersion version)
        {
            var document = _db.Documents.First(d => d.Id == version.DocumentId);

            _currentFileName = document.FileName;

            FireQueryingEvent();

            var source = await GetPathToVersionFileAsync(version);

            var destination = Path.Combine(GetTempDirectory(), _currentFileName);
            
            await DecryptAsync(source, destination);

            var process = Process.Start(destination);

            await process.WaitForExitAsync();

            File.Delete(destination);
        }

        /// <summary>
        /// Adds the specified source file to storage and attaches it to the specified entity
        /// </summary>
        /// <param name="source">The file to store</param>
        /// <param name="entity">The entity to attach to</param>
        /// <returns></returns>
        public async Task AddAsync(string source, object entity)
        {
            _currentFileName = Path.GetFileName(source);

            FireQueryingEvent();
            
            bool isExisting = await IsExistingDocumentAsync(entity);
            
            if (isExisting)
            {
                
            }

            var hash = await ComputeHashValueAsync(source);

            FireQueryingEvent();

            var document = CreateDocumentForEntity(entity);

            var version = new DocumentVersion
            {
                Changes = "First version",
                CreatedBy = _employee,
                CreatedAt = DateTime.Now,
                Document = document,
                Hash = hash
            };

            _db.Documents.Add(document);
            _db.DocumentVersions.Add(version);

            await _db.SaveChangesAsync();

            var destination = await GetPathToVersionFileAsync(version);

            await EncryptAsync(source, destination);

            FireCompletedEvent();
        }

        public async Task DeleteAsync(Document document)
        {
            
        }

        public async Task CheckOutAsync(Document document)
        {
            
        }

        public async Task CheckInAsync(Document document)
        {
            
        }

        public void PurgeTempDirectory()
        {
            var temp = GetTempDirectory();

            var files = Directory.GetFiles(temp);

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch { } // empty catch as files may be locked. should clean them up some other time the method is called 
            }
        }

        private string GetTempDirectory()
        {
            return Path.Combine(Path.GetTempPath(), "purge");
        }

        private Document CreateDocumentForEntity(object entity)
        {
            var document = new Document {FileName = _currentFileName };

            if (entity is DocumentFolder)
            {
                var id = (entity as DocumentFolder).Id;

                document.DocumentFolderId = id;
            }
            else if (entity is Part)
            {
                var id = (entity as Part).Id;

                document.PartId = id;
            }
            else if (entity is PartVersion)
            {
                var id = (entity as PartVersion).Id;

                document.PartVersionId = id;
            }

            return document;
        }

        private async Task<bool> IsExistingDocumentAsync(object entity)
        {
            if (entity is DocumentFolder)
            {
                var id = (entity as DocumentFolder).Id;

                return await _db.Documents.AnyAsync(d =>
                               d.DocumentFolderId == id &&
                               d.FileName.Equals(_currentFileName, StringComparison.OrdinalIgnoreCase));
            }

            if (entity is Part)
            {
                var id = (entity as Part).Id;

                return await _db.Documents.AnyAsync(d =>
                    d.PartId == id &&
                    d.FileName.Equals(_currentFileName, StringComparison.OrdinalIgnoreCase));
            }

            if (entity is PartVersion)
            {
                var id = (entity as PartVersion).Id;

                return await _db.Documents.AnyAsync(d =>
                    d.PartVersionId == id &&
                    d.FileName.Equals(_currentFileName, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        private async Task<string> GetEntityStoragePathAsync(object entity)
        {
            if (entity is Part)
            {
                var part = entity as Part;

                return $"cid_{part.CustomerId}\\pid_{part.Id}";
            }

            if (entity is PartVersion)
            {
                var version = entity as PartVersion;

                var part = _db.Parts.SingleOrDefault(p => p.Id == version.PartId);

                var partDir = await GetEntityStoragePathAsync(part);

                return $"{partDir}\\vid_{version.Id}";
            }

            throw new NotImplementedException(
                $"Attaching to an entity of the type {entity.GetType().Name} has not been implemented!");
        }

        private async Task<string> GetPathToVersionFileAsync(DocumentVersion version)
        {
            var document = await _db.Documents.FirstAsync(d => d.Id == version.DocumentId);

            object entity = null;

            if (document.PartId.HasValue)
            {
                entity = await _db.Parts.FirstAsync(p => p.Id == document.PartId);
            }
            else if (document.PartVersionId.HasValue)
            {
                entity = await _db.PartVersions.FirstAsync(v => v.Id == document.PartVersionId);
            }
            else if (document.DocumentFolderId.HasValue)
            {
                entity = await _db.DocumentFolders.FirstAsync(f => f.Id == document.DocumentFolderId);
            }

            var entityStoragePath = await GetEntityStoragePathAsync(entity);

            return Path.Combine(entityStoragePath, version.Id.ToString());
        }
        
        private async Task<string> ComputeHashValueAsync(string source)
        {
            return await Task.Factory.StartNew(() =>
            {
                using (var murmur3 = new Murmur3())
                {
                    using (var fs = File.Open(source, FileMode.Open, FileAccess.Read))
                    {
                        using (var hashProgressStream = new ProgressStream(fs))
                        {
                            hashProgressStream.ProgressChanged += HashProgressStream_ProgressChanged;
                            var hash = murmur3.ComputeHash(hashProgressStream);

                            return hash.ToHexString();
                        }
                    }
                }
            });
        }

        private async Task EncryptAsync(string source, string destination)
        {
            var aes = new AESEncryptionProvider();
            aes.ProgressChanged += Encryption_ProgressChanged;

            await aes.EncryptFileAsync(source, destination, SecureDataStorage.EncryptionPassword);
        }

        private async Task DecryptAsync(string source, string destination)
        {
            var aes = new AESEncryptionProvider();
            aes.ProgressChanged += Decryption_ProgressChanged;

            await aes.DecryptFileAsync(source, destination, SecureDataStorage.EncryptionPassword);
        }

        private void Decryption_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var args = new DocumentTransferEventArgs(_currentFileName, DocumentTransferStep.Decrypting,
                e.ProgressPercentage);

            OnTransferProgress(args);
        }

        private void Encryption_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var args = new DocumentTransferEventArgs(_currentFileName, DocumentTransferStep.Encrypting,
                e.ProgressPercentage);

            OnTransferProgress(args);
        }

        private void HashProgressStream_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var args = new DocumentTransferEventArgs(_currentFileName, DocumentTransferStep.ComputingHash,
                e.ProgressPercentage);

            OnTransferProgress(args);
        }

        private void FireCompletedEvent()
        {
            OnTransferProgress(new DocumentTransferEventArgs(_currentFileName, DocumentTransferStep.Complete, 100));
        }

        private void FireQueryingEvent()
        {
            OnTransferProgress(new DocumentTransferEventArgs(_currentFileName, DocumentTransferStep.Querying, 0));
        }

        protected virtual void OnTransferProgress(DocumentTransferEventArgs e)
        {
            TransferProgress?.Invoke(this, e);
        }
    }
}