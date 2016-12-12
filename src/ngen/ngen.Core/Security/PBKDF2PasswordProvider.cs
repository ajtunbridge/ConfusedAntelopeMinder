#region Using directives

using System;
using System.Security.Cryptography;

#endregion

namespace ngen.Core.Security
{
    public class PBKDF2PasswordProvider : IPasswordProvider
    {
        private const int Iterations = 46125;

        public string HashPassword(string plainPassword)
        {
            var salt = new byte[32];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            
            var hash = ComputeHash(plainPassword, salt);

            var retval = $"{hash}.{Convert.ToBase64String(salt)}";

            return retval;
        }

        public bool Verify(string plainPassword, string hash)
        {
            if (plainPassword == null || hash == null)
            {
                return false;
            }

            var saltString = hash.Substring(hash.IndexOf(".", StringComparison.Ordinal) + 1);

            var saltBytes = Convert.FromBase64String(saltString);

            var newHash = ComputeHash(plainPassword, saltBytes);

            var passwordHash = hash.Substring(0, hash.IndexOf(".", StringComparison.Ordinal));

            return newHash.Equals(passwordHash);
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