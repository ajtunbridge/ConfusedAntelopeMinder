using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ngen.Data.Model
{
    public class WorkCentre
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        public string Caption { get; set; }

        public int WorkCentreGroupId { get; set; }

        [InverseProperty("WorkCentres")]
        public WorkCentreGroup WorkCentreGroup { get; set; }

        [NotMapped]
        public string PhotoAddress => $"WorkCentre:{Id}";

        public virtual ICollection<Operation> AssignedOperations { get; set; }
    }
}