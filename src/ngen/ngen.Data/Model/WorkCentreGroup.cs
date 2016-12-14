using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ngen.Data.Model
{
    public class WorkCentreGroup
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(-1)]
        public string Description { get; set; }

        public decimal? DefaultHourlyRate { get; set; }

        /// <summary>
        ///     Used for storing the id values of any parent groups in the format '\{id1}\{id2}\{id3}\....'
        /// </summary>
        public string Lineage { get; set; }

        /// <summary>
        /// Any groups below this group in the tree will have a Lineage starting with this value
        /// </summary>
        [NotMapped]
        public string ChildLineage => $"{Lineage}{Id}";

        public int? ParentGroupId { get; set; }

        [InverseProperty("Children")]
        public virtual WorkCentreGroup Parent { get; set; }

        public virtual ICollection<WorkCentreGroup> Children { get; set; }
        
        public virtual ICollection<WorkCentre> WorkCentres { get; set; }

        public virtual ICollection<Operation> AssignedOperations { get; set; }
    }
}