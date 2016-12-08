#region Using directives

using System;
using System.Security.Cryptography;

#endregion

namespace ngen.Core.Security
{
    public class PBKDF2PasswordProvider : IPasswordProvider
    {
        private const int Iterations = 25000;

        public EncryptedPassword EncryptPassword(string plainPassword)
        {
            var hashed = new EncryptedPassword();

            // saw an article somewhere about using a GUID as a salt and it seemed like a nice solution.
            var salt = Guid.NewGuid().ToByteArray();

            hashed.Hash = ComputeHash(plainPassword, salt);
            hashed.Salt = Convert.ToBase64String(salt);

            return hashed;
        }

        public bool AreEqual(string plainPassword, string hash, string salt)
        {
            if (plainPassword == null || hash == null || salt == null)
            {
                return false;
            }

            var saltBytes = Convert.FromBase64String(salt);

            var newHash = ComputeHash(plainPassword, saltBytes);

            return newHash.Equals(hash);
        }

        private string ComputeHash(string text, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(text, salt, Iterations))
            {
                var key = pbkdf2.GetBytes(64);
                return Convert.ToBase64String(key);
            }
        }
    }
}