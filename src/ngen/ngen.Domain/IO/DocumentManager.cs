using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ngen.Core.IO;
using ngen.Core.Security;
using ngen.Data.Model;
using ngen.Domain.Enum;
using ngen.Domain.EventArgs;
using ngen.Domain.Security;

namespace ngen.Domain.IO
{
    public class DocumentManager
    {
        private readonly Employee _employee;
        private string _currentFileName;

        public DocumentManager(Employee employee)
        {
            _employee = employee;
        }

        public event EventHandler<DocumentTransferEventArgs> TransferProgress;


        public async Task AddAsync(string source, PartVersion partVersion)
        {
            
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

        private async Task Encrypt(string source, string destination)
        {
            var aes = new AESEncryptionProvider();
            aes.ProgressChanged += Encryption_ProgressChanged;

            await aes.EncryptFileAsync(source, destination, SecureDataStorage.EncryptionPassword);
        }

        private async Task Decrypt(string source, string destination)
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

        protected virtual void OnTransferProgress(DocumentTransferEventArgs e)
        {
            TransferProgress?.Invoke(this, e);
        }
    }
}