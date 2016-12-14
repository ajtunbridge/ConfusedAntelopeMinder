using System;
using ngen.Data;
using ngen.Data.Model;

namespace ngen.Domain.Security
{
    public class SystemPermissionException : Exception
    {
        public SystemPermissionException(string message, Employee employee, SystemPermission permission) : base(message)
        {
            Employee = employee;
            Permission = permission;
            ThrownAt = DateTime.Now;
        }

        public DateTime ThrownAt { get; private set; }

        public Employee Employee { get; private set; }

        public SystemPermission Permission { get; private set; }
    }
}