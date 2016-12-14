using System.Data.Entity;
using System.Threading.Tasks;
using ngen.Data;
using ngen.Data.Blobs;
using ngen.Data.Model;

namespace ngen.Domain.Security
{
    /// <summary>
    /// Provides security methods to ensure employees have correct permissions and access rights to the system
    /// </summary>
    public static class SecurityManager
    {
        /// <summary>
        /// Checks if the employee has the specified permission and throws a <see cref="SystemPermissionException"/> if they do not
        /// </summary>
        /// <param name="permission">The permission to check for</param>
        /// <param name="employee">The employee to check</param>
        /// <exception cref="SystemPermissionException">Throws exception if employee doesn't have the specified permission</exception>
        /// <returns></returns>
        public static async Task EnforceAsync(SystemPermission permission, Employee employee)
        {
            using (var context = new ngenDbContext())
            {
                var role = await context.SystemRoles.FirstAsync(r => r.Id == employee.SystemRoleId);

                var perms = SystemRolePermissions.FromBytes(role.Permissions);

                if (!perms.Has(permission))
                    throw new SystemPermissionException("You do not have permission to do this!", employee,
                        permission);
            }
        }

        /// <summary>
        /// Checks if the employee has read access to the specified document and throws a <see cref="DocumentAccessException"/> if they do not
        /// </summary>
        /// <param name="documentId">The id of the document to check</param>
        /// <param name="employee">The employee to check read access for</param>
        /// <exception cref="DocumentAccessException">Throws exception if employee doesn't have the read access to the document</exception>
        /// <returns></returns>
        public static async Task EnforceReadAccessAsync(int documentId, Employee employee)
        {
            await EnforceAccessRightAsync(documentId, employee, AccessRight.Read);
        }

        /// <summary>
        /// Checks if the employee has write access to the specified document and throws a <see cref="DocumentAccessException"/> if they do not
        /// </summary>
        /// <param name="documentId">The id of the document to check</param>
        /// <param name="employee">The employee to check write access for</param>
        /// <exception cref="DocumentAccessException">Throws exception if employee doesn't have the write access to the document</exception>
        /// <returns></returns>
        public static async Task EnforceWriteAccessAsync(int documentId, Employee employee)
        {
            await EnforceAccessRightAsync(documentId, employee, AccessRight.ReadWrite);
        }

        private static async Task EnforceAccessRightAsync(int documentId, Employee employee, AccessRight right)
        {
            using (var context = new ngenDbContext())
            {
                var document = await context.Documents.FirstAsync(d => d.Id == documentId);

                var permissions = document.Permissions == null
                    ? null
                    : DocumentPermissions.FromBytes(document.Permissions);

                if (permissions == null)
                    return;

                switch (right)
                {
                    case AccessRight.Read:
                        if (!permissions.CanRead(employee))
                            throw new DocumentAccessException("You do not have read access for this document", document,
                                employee);
                        break;
                    case AccessRight.ReadWrite:
                        if (!permissions.CanWrite(employee))
                            throw new DocumentAccessException("You do not have write access to this document", document,
                                employee);
                        break;
                }
            }
        }
    }
}