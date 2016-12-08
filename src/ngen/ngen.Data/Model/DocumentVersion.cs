#region Using directives

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace ngen.Data.Model
{
    public class DocumentVersion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(32)]
        [Column(TypeName = "nchar")]
        public string Hash { get; set; }

        public string Changes { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedById { get; set; }

        public int DocumentId { get; set; }

        public Employee CreatedBy { get; set; }

        public Document Document { get; set; }
    }
}