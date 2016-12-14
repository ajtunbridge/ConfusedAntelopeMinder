#region Using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ngen.Core.Security;

#endregion

namespace ngen.Core.Tests
{
    [TestClass]
    public class PasswordProviderTests
    {
        [TestMethod]
        public void Computed_PBKDF2_hash_is_correct_length()
        {
            IPasswordProvider provider = new PBKDF2PasswordProvider();

            var password = provider.HashPassword("plaintextpassword");

            Assert.AreEqual(password.Length, 133);
        }

        [TestMethod]
        public void Computed_PBKDF2_hashes_match()
        {
            IPasswordProvider provider = new PBKDF2PasswordProvider();

            var password = provider.HashPassword("plaintextpassword");

            Assert.IsTrue(provider.Verify("plaintextpassword", password));
        }

        [TestMethod]
        public void Computed_BCrypt_hash_is_correct_length()
        {
            IPasswordProvider provider = new BCryptPasswordProvider();

            var password = provider.HashPassword("plaintextpassword");

            Assert.AreEqual(password.Length, 60);
        }

        [TestMethod]
        public void Computed_BCrypt_hashes_match()
        {
            IPasswordProvider provider = new BCryptPasswordProvider();

            var password = provider.HashPassword("plaintextpassword");

            Assert.IsTrue(provider.Verify("plaintextpassword", password));
        }
    }
}