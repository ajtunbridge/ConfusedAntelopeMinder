﻿#region Using directives

using ngen.Core.IO;

#endregion

namespace ngen.Data.Blobs
{
    public sealed class SystemRolePermissions
    {
        private readonly SystemPermission _permissions = SystemPermission.NotSet;

        public void Grant(SystemPermission permission)
        {
            _permissions.Add(permission);
        }

        public void Deny(SystemPermission permission)
        {
            _permissions.Remove(permission);
        }

        public bool Has(SystemPermission permission)
        {
            return _permissions.Has(permission);
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