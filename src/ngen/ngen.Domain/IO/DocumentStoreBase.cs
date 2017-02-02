using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ngen.Core.IO;
using ngen.Core.Security;
using ngen.Data;
using ngen.Data.Blobs;
using ngen.Data.Model;
using ngen.Domain.EventArgs;

namespace ngen.Domain.IO
{
    public abstract class DocumentStoreBase : IDisposable
    {
        private const string TempDirectoryName = "NjSS.1x5";
        private readonly bool _createdDataContext;
        protected readonly ngenDbContext DataContext;
        protected string CurrentFileName;

        protected DocumentStoreBase(Employee currentEmployee)
        {
            DataContext = new ngenDbContext();
            CurrentEmployee = currentEmployee;

            _createdDataContext = true;
        }

        protected DocumentStoreBase(ngenDbContext dataContext, Employee currentEmployee)
        {
            DataContext = dataContext;
            CurrentEmployee = currentEmployee;

            _createdDataContext = false;
        }

        public Employee CurrentEmployee { get; set; }

        /// <summary>
        ///     The directory in which to store any temporary files that will be deleted when finished with
        /// </summary>
        protected string TempFileDirectory => Path.Combine(Path.GetTempPath(), TempDirectoryName);

        public void Dispose()
        {
            // dispose of context if it was not passed in through the constructor
            if (_createdDataContext)
                DataContext.Dispose();
        }

        public event EventHandler<DocumentTransferEventArgs> TransferProgress;

        public async void PurgeTempFiles()
        {
            // fire and forget. any exceptions will be swallowed.
            await Task.Factory.StartNew(() =>
            {
                var files = Directory.GetFiles(TempFileDirectory);

                foreach (var file in files)
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        // empty catch in case files are open and in use. will clean them up some other time the method is called 
                    }
            });
        }

        protected async Task<bool> IsExistingDocumentAsync(object entity)
        {
            if (entity is DocumentFolder)
            {
                var id = (entity as DocumentFolder).Id;

                return await DataContext.Documents.AnyAsync(d =>
                    (d.DocumentFolderId == id) &&
                    d.FileName.Equals(CurrentFileName, StringComparison.OrdinalIgnoreCase));
            }

            if (entity is Part)
            {
                var id = (entity as Part).Id;

                return await DataContext.Documents.AnyAsync(d =>
                    (d.PartId == id) &&
                    d.FileName.Equals(CurrentFileName, StringComparison.OrdinalIgnoreCase));
            }

            if (entity is PartVersion)
            {
                var id = (entity as PartVersion).Id;

                return await DataContext.Documents.AnyAsync(d =>
                    (d.PartVersionId == id) &&
                    d.FileName.Equals(CurrentFileName, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        protected async Task<string> ComputeHashValueAsync(string source)
        {
            if (!File.Exists(source))
            {
                throw new FileNotFoundException($"Unable to locate file {source}");
            }

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
        
        protected Document CreateDocumentForEntity(object entity)
        {
            var document = new Document {FileName = CurrentFileName};

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

        protected async Task<DocumentVersion> CreateNewDocumentVersionAsync(string source, Document document,
            string changes)
        {
            var hash = await ComputeHashValueAsync(source);

            var version = new DocumentVersion
            {
                Changes = changes,
                CreatedById = CurrentEmployee.Id,
                CreatedAt = DateTime.Now,
                Document = document,
                Hash = hash
            };

            return version;
        }

        protected async Task<DocumentVersion> GetLatestVersionAsync(Document document)
        {
            return await DataContext.DocumentVersions
                .Where(v => v.DocumentId == document.Id)
                .OrderByDescending(v => v.CreatedAt)
                .FirstAsync();
        }

        protected async Task<DocumentVersion> GetObsoleteVersionAsync(Document document)
        {
            var versions = DataContext.DocumentVersions
                .Where(v => v.DocumentId == document.Id)
                .OrderBy(v => v.CreatedAt);

            var clientSetting = await DataContext.ClientSettings.FirstAsync();

            if (await versions.CountAsync() > clientSetting.MaximumVersionCount)
                return await versions.FirstAsync();

            return null;
        }

        protected async Task MarkAsCheckedInAsync(Document document)
        {
            document.CheckedOutAt = null;
            document.CheckedOutById = null;
            await DataContext.SaveChangesAsync();
        }

        protected async Task MarkAsCheckedOutAsync(Document document)
        {
            document.CheckedOutAt = DateTime.Now;
            document.CheckedOutBy = CurrentEmployee;
            await DataContext.SaveChangesAsync();
        }

        protected async Task DeleteDocumentRecordsAsync(Document document)
        {
            var documentVersions = await DataContext.DocumentVersions.Where(v => v.DocumentId == document.Id).ToListAsync();

            foreach (var version in documentVersions)
            {
                DataContext.DocumentVersions.Remove(version);
            }

            DataContext.Documents.Remove(document);

            await DataContext.SaveChangesAsync();
        }
        
        private void HashProgressStream_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var args = new DocumentTransferEventArgs(CurrentFileName, DocumentTransferStatus.ComputingHash,
                e.ProgressPercentage);

            OnTransferProgress(args);
        }

        protected void OnTransferProgress(DocumentTransferEventArgs e)
        {
            TransferProgress?.Invoke(this, e);
        }

        
    }
}