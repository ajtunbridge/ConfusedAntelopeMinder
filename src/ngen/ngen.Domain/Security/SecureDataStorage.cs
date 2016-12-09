#region Using directives

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace ngen.Domain.Security
{
    public static class SecureDataStorage
    {
        private const string EncryptionKeyFileName = "encryption_key";
        private const string FileShareCredentialsFileName = "file_share_credentials";
        private static readonly IsolatedStorageFile IsoStorageFile;

        static SecureDataStorage()
        {
            IsoStorageFile = IsolatedStorageFile.GetMachineStoreForAssembly();
        }


        public static string EncryptionPassword
        {
            get { return GetEncryptionPassword(); }
            set { SetEncryptionPassword(value); }
        }

        public static NetworkCredential FileShareCredentials
        {
            get { return GetFileShareCredentials(); }
            set { SetFileShareCredentials(value); }
        }


        private static void SetFileShareCredentials(NetworkCredential credentials)
        {
            var pipeDelimitedValues = $"{credentials.UserName}|{credentials.Password}|{credentials.Domain}";

            var bytes = Encoding.UTF8.GetBytes(pipeDelimitedValues);

            EncryptAndWriteToFile(bytes, FileShareCredentialsFileName);
        }

        private static NetworkCredential GetFileShareCredentials()
        {
            var bytes = ReadAndDecryptFromFile(FileShareCredentialsFileName);

            var pipeDelimitedValues = Encoding.UTF8.GetString(bytes);

            var split = pipeDelimitedValues.Split(new[] {"|"}, StringSplitOptions.None);

            var credentials = new NetworkCredential(split[0], split[1], split[2]);

            return credentials;
        }

        private static void SetEncryptionPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);

            EncryptAndWriteToFile(bytes, EncryptionKeyFileName);
        }

        private static string GetEncryptionPassword()
        {
            var bytes = ReadAndDecryptFromFile(EncryptionKeyFileName);
            
            return Encoding.UTF8.GetString(bytes);
        }


        private static void EncryptAndWriteToFile(byte[] bytes, string fileName)
        {
            var entropy = new byte[20];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }

            var encryptedData = ProtectedData.Protect(bytes, entropy, DataProtectionScope.CurrentUser);
            
            using (var isoStream = new IsolatedStorageFileStream(fileName, FileMode.Create, IsoStorageFile))
            {
                using (var writer = new StreamWriter(isoStream))
                {
                    writer.WriteLine(Convert.ToBase64String(encryptedData));
                    writer.WriteLine(Convert.ToBase64String(entropy));
                }
            }
        }

        private static byte[] ReadAndDecryptFromFile(string fileName)
        {
            if (!IsoStorageFile.FileExists(fileName))
                return null;

            using (var isoStream = new IsolatedStorageFileStream(fileName, FileMode.Open, IsoStorageFile))
            {
                using (var reader = new StreamReader(isoStream))
                {
                    var cipherText = Convert.FromBase64String(reader.ReadLine());
                    var entropy = Convert.FromBase64String(reader.ReadLine());

                    var clearBytes = ProtectedData.Unprotect(cipherText, entropy, DataProtectionScope.CurrentUser);

                    return clearBytes;
                }
            }
        }
    }
}