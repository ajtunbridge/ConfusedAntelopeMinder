#region Using directives

using System.Collections.Generic;
using ngen.Core.IO;
using ngen.Data.Model;

#endregion

namespace ngen.Data.Blobs
{
    public class DocumentPermissions
    {
        private readonly Dictionary<int, AccessRight> _employeeRights = new Dictionary<int, AccessRight>();
        private readonly Dictionary<int, AccessRight> _systemRoleRights = new Dictionary<int, AccessRight>();

        public void Set(Employee employee, AccessRight right)
        {
            if (_employeeRights.ContainsKey(employee.Id))
            {
                _employeeRights[employee.Id] = right;
            }
            else
            {
                _employeeRights.Add(employee.Id, right);
            }
        }

        public void Set(SystemRole role, AccessRight right)
        {
            if (_systemRoleRights.ContainsKey(role.Id))
            {
                _systemRoleRights[role.Id] = right;
            }
            else
            {
                _systemRoleRights.Add(role.Id, right);
            }
        }

        public bool CanRead(Employee employee)
        {
            var employeeRights = GetEmployeeRights(employee.Id);

            if (employeeRights == AccessRight.NotSet)
            {
                var roleRights = GetRoleRights(employee.SystemRoleId);

                if (roleRights == AccessRight.NotSet)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanWrite(Employee employee)
        {
            var employeeRights = GetEmployeeRights(employee.Id);

            if (employeeRights == AccessRight.NotSet)
            {
                var roleRights = GetRoleRights(employee.SystemRoleId);

                return roleRights == AccessRight.ReadWrite;
            }

            return employeeRights == AccessRight.ReadWrite;
        }

        private AccessRight GetRoleRights(int roleId)
        {
            if (_systemRoleRights.ContainsKey(roleId))
            {
                return _systemRoleRights[roleId];
            }

            return AccessRight.NotSet;
        }

        private AccessRight GetEmployeeRights(int employeeId)
        {
            if (_employeeRights.ContainsKey(employeeId))
            {
                return _employeeRights[employeeId];
            }

            return AccessRight.NotSet;
        }

        public byte[] ToBytes()
        {
            return BlobHandler.Compress(this);
        }

        public static DocumentPermissions FromBytes(byte[] blob)
        {
            return BlobHandler.Decompress<DocumentPermissions>(blob);
        }
    }
}