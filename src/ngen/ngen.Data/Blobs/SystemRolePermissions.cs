#region Using directives

using System;
using ngen.Core.IO;

#endregion

namespace ngen.Data.Blobs
{
    [Serializable]
    public sealed class SystemRolePermissions
    {
        private SystemPermission _permissions = SystemPermission.NotSet;

        public void Grant(SystemPermission permission)
        {
            _permissions = _permissions.Add(permission);
        }

        public void Deny(SystemPermission permission)
        {
            _permissions = _permissions.Remove(permission);
        }

        public bool Has(SystemPermission permission)
        {
            // administrator permission overrides all others
            return _permissions.Has(SystemPermission.Administrator) || _permissions.Has(permission);
        }

        public byte[] ToBytes()
        {
            return BlobHandler.Compress(this);
        }

        public static SystemRolePermissions FromBytes(byte[] bytes)
        {
            return BlobHandler.Decompress<SystemRolePermissions>(bytes);
        }
    }
}