#region Using directives

using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ngen.Core.IO;

#endregion

namespace ngen.Core.Security
{
    public sealed class AESEncryptionProvider : IEncryptionProvider
    {
        private const int Iterations = 1981;

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        /// <summary>Decrypt a file.</summary>
        /// <remarks>
        ///     NB: "Padding is invalid and cannot be removed." is the Universal CryptoServices error.  Make sure the
        ///     password, salt and iterations are correct before getting nervous.
        /// </remarks>
        /// <param name="srcFile">The full path and name of the file to be decrypted.</param>
        /// <param name="destFile">The full path and name of the file to be output.</param>
        /// <param name="password">The password for the decryption.</param>
        public async Task DecryptFileAsync(string srcFile, string destFile, string password)
        {
            using (var destination = new FileStream(destFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var aes = new AesManaged();
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Mode = CipherMode.CBC;

                using (var source = new FileStream(srcFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var progress = new ProgressStream(source, destFile))
                    {
                        // read the salt from the start of the file
                        byte[] salt;

                        using (var br = new BinaryReader(source, Encoding.Default, true))
                        {
                            salt = br.ReadBytes(16);
                        }

                        // derive key and IV from supplied password
                        var key = new Rfc2898DeriveBytes(password, salt, Iterations);
                        aes.Key = key.GetBytes(aes.KeySize/8);
                        aes.IV = key.GetBytes(aes.BlockSize/8);
                        aes.Mode = CipherMode.CBC;

                        var transform = aes.CreateDecryptor(aes.Key, aes.IV);

                        using (var cryptoStream = new CryptoStream(destination, transform, CryptoStreamMode.Write))
                        {
                            progress.ProgressChanged += Progress_ProgressChanged;

                            await progress.CopyToAsync(cryptoStream);
                        }
                    }
                }
            }
        }

        /// <summary>Encrypt a file.</summary>
        /// <param name="srcFile">The full path and name of the file to be encrypted.</param>
        /// <param name="destFile">The full path and name of the file to be output.</param>
        /// <param name="password">The password for the encryption.</param>
        public async Task EncryptFileAsync(string srcFile, string destFile, string password)
        {
            var destDir = Path.GetDirectoryName(destFile);

            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            var aes = new AesManaged();
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;

            // generate random 16 byte salt value
            var salt = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(salt);
            
            // derive key and IV from supplied password
            var key = new Rfc2898DeriveBytes(password, salt, Iterations);
            aes.Key = key.GetBytes(aes.KeySize/8);
            aes.IV = key.GetBytes(aes.BlockSize/8);
            aes.Mode = CipherMode.CBC;

            // prepend salt value to start of file
            using (var dest = new FileStream(destFile, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (var bw = new BinaryWriter(dest))
                {
                    bw.Write(salt);
                }
            }

            // encrypt the source file to the destination
            var transform = aes.CreateEncryptor(aes.Key, aes.IV);

            using (var destination = new FileStream(destFile, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                using (var cryptoStream = new CryptoStream(destination, transform, CryptoStreamMode.Write))
                {
                    using (var source = new FileStream(srcFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var progress = new ProgressStream(source, srcFile))
                        {
                            progress.ProgressChanged += Progress_ProgressChanged;
                            await progress.CopyToAsync(cryptoStream);
                        }
                    }
                }
            }
        }

        private void Progress_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            OnProgressChanged(new ProgressChangedEventArgs(e.ProgressPercentage, null));
        }

        private void OnProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }
}