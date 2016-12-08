using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ngen.Data.Blobs;
using ngen.Data.Model;

namespace ngen.Data.Tests
{
    [TestClass]
    public class BlobTests
    {
        [TestMethod]
        public void Can_add_a_system_role_permission()
        {
            var perms = new SystemRolePermissions();

            perms.Grant(SystemPermission.Administrator);

            Assert.IsTrue(perms.Has(SystemPermission.Administrator));
        }

        [TestMethod]
        public void Can_retrieve_available_system_role_permissions()
        {
            var perms = new SystemRolePermissions();
            
            perms.Grant(SystemPermission.ManageSupplierAccounts);
            perms.Grant(SystemPermission.ViewFinancialDetails);

            Assert.IsTrue(perms.Has(SystemPermission.ManageSupplierAccounts));

            Assert.IsFalse(perms.Has(SystemPermission.CheckOutDocuments));
        }

        [TestMethod]
        public void Administrator_system_role_permission_overrides_others()
        {
            var perms = new SystemRolePermissions();

            perms.Grant(SystemPermission.Administrator);

            Assert.IsTrue(perms.Has(SystemPermission.ManageSupplierAccounts));
            Assert.IsTrue(perms.Has(SystemPermission.ManageDocuments));
            Assert.IsTrue(perms.Has(SystemPermission.ManageCustomerAccounts));
            Assert.IsTrue(perms.Has(SystemPermission.CheckOutDocuments));
            Assert.IsTrue(perms.Has(SystemPermission.ManageParts));
            Assert.IsTrue(perms.Has(SystemPermission.ViewFinancialDetails));
        }


        [TestMethod]
        public void Can_grant_employees_role_read_only_access_to_document()
        {
            var role = new SystemRole {Id = 1, Name = "Test Role"};
            var employee = new Employee {Id = 1, SystemRoleId = role.Id};

            var docPermissions = new DocumentPermissions();

            Assert.IsFalse(docPermissions.CanRead(employee));

            docPermissions.Set(role, AccessRight.Read);

            Assert.IsTrue(docPermissions.CanRead(employee));
        }

        [TestMethod]
        public void Can_grant_employees_role_write_access_to_document()
        {
            var role = new SystemRole { Id = 1, Name = "Test Role" };
            var employee = new Employee { Id = 1, SystemRoleId = role.Id };

            var docPermissions = new DocumentPermissions();

            Assert.IsFalse(docPermissions.CanWrite(employee));

            docPermissions.Set(role, AccessRight.ReadWrite);

            Assert.IsTrue(docPermissions.CanWrite(employee));
        }
    }
}