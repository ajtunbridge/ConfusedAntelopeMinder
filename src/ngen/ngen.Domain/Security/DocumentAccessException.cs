using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ngen.Data.Model;

namespace ngen.Domain.Security
{
    public class DocumentAccessException : Exception
    {
        public Document Document { get; private set; }

        public Employee Employee { get; private set; }

        public DateTime ThrownAt { get; private set; }

        public DocumentAccessException(string message, Document document, Employee employee) : base(message)
        {
            Document = document;
            Employee = employee;
            ThrownAt = DateTime.Now;
        }
    }
}
