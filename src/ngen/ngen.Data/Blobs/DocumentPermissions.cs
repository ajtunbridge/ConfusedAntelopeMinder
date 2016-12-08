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
            var rights = GetRights(employee);

            return rights != AccessRight.None;
        }

        public bool CanWrite(Employee employee)
        {
            var rights = GetRights(employee);

            return rights == AccessRight.ReadWrite;
        }

        private AccessRight GetRights(Employee employee)
        {
            if (_systemRoleRights.ContainsKey(employee.SystemRoleId))
            {
                return _systemRoleRights[employee.SystemRoleId];
            }

            if (_employeeRights.ContainsKey(employee.Id))
            {
                return _employeeRights[employee.Id];
            }

            return AccessRight.None;
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