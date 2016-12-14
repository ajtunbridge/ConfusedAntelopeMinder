using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ngen.Data.Model
{
    public class ProductionMethod
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(-1)]
        public string Description { get; set; }

        [Required]
        public byte Rating { get; set; } = 1;

        public DateTime CreatedAt { get; set; }

        public bool IsProven { get; set; } = true;

        public int CreatedById { get; set; }

        public int PartVersionId { get; set; }

        public Employee CreatedBy { get; set; }

        [InverseProperty("ProductionMethods")]
        public PartVersion PartVersion { get; set; }

        public virtual ICollection<Operation> Operations { get; set; }
    }
}