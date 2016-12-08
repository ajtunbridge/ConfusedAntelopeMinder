#region Using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ngen.Core.Security;

#endregion

namespace ngen.Core.Tests
{
    [TestClass]
    public class SecureDataStorageTests
    {
        [TestMethod]
        public void Stored_Encryption_Password_Returns_Correct_Value()
        {
            var password = "happyhappyhappyjoyjoyjoy";

            SecureDataStorage.EncryptionPassword = password;

            Assert.AreEqual(password, SecureDataStorage.EncryptionPassword);
        }
    }
}