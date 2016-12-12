using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngen.Data.Model
{
    public class DocumentFolder
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(-1)]
        public string Description { get; set; }

        public int? ParentId { get; set; }

        public int? ClientSettingId { get; set; }

        public int? CustomerId { get; set; }

        public int? EmployeeId { get; set; }

        public int? PartId { get; set; }

        public int? PartVersionId { get; set; }

        public int? SupplierId { get; set; }
        
        public DocumentFolder Parent { get; set; }

        public ClientSetting ClientSetting { get; set; }

        public Customer Customer { get; set; }

        public Employee Employee { get; set; }

        public Part Part { get; set; }

        public PartVersion PartVersion { get; set; }

        public Supplier Supplier { get; set; }
    }
}
