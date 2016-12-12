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
        public void Computed_PBKDF2_Hash_Is_Correct_Length()
        {
            IPasswordProvider provider = new PBKDF2PasswordProvider();

            var password = provider.HashPassword("plaintextpassword");

            Assert.AreEqual(password.Hash.Length, 88);
        }

        [TestMethod]
        public void Computed_PBKDF2_Hashes_Match()
        {
            IPasswordProvider provider = new PBKDF2PasswordProvider();

            var password = provider.HashPassword("plaintextpassword");

            Assert.IsTrue(provider.Verify("plaintextpassword", password.Hash, password.Salt));
        }
    }
}