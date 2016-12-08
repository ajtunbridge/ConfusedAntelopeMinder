#region Using directives

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace ngen.Core.Security
{
    public static class SecureDataStorage
    {
        private static readonly IsolatedStorageFile IsoStorageFile;
        private static readonly string EncryptionKeyFileName = "enc_key";

        static SecureDataStorage()
        {
            IsoStorageFile = IsolatedStorageFile.GetMachineStoreForAssembly();
        }

        public static string EncryptionPassword
        {
            get { return GetEncryptionPassword(); }
            set { SetEncryptionPassword(value); }
        }

        private static void SetEncryptionPassword(string password)
        {
            var plaintext = Encoding.UTF8.GetBytes(password);

            var entropy = new byte[20];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }

            var ciphertext = ProtectedData.Protect(plaintext, entropy, DataProtectionScope.CurrentUser);

            if (!IsoStorageFile.FileExists(EncryptionKeyFileName))
            {
                using (
                    var isoStream = new IsolatedStorageFileStream(EncryptionKeyFileName, FileMode.CreateNew,
                        IsoStorageFile))
                {
                    using (var writer = new StreamWriter(isoStream))
                    {
                        writer.WriteLine(Convert.ToBase64String(ciphertext));
                        writer.WriteLine(Convert.ToBase64String(entropy));
                    }
                }
            }
        }

        private static string GetEncryptionPassword()
        {
            if (!IsoStorageFile.FileExists(EncryptionKeyFileName))
            {
                return null;
            }

            using (var isoStream = new IsolatedStorageFileStream(EncryptionKeyFileName, FileMode.Open, IsoStorageFile))
            {
                using (var reader = new StreamReader(isoStream))
                {
                    var cipherText = Convert.FromBase64String(reader.ReadLine());
                    var entropy = Convert.FromBase64String(reader.ReadLine());

                    var clearBytes = ProtectedData.Unprotect(cipherText, entropy, DataProtectionScope.CurrentUser);

                    return Encoding.UTF8.GetString(clearBytes);
                }
            }
        }
    }
}