using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ngen.Data.Model
{
    public class Operation
    {
        public int Id { get; set; }

        public byte Sequence { get; set; }

        [MaxLength(-1)]
        public string Description { get; set; }

        [Required]
        public double SetupTime { get; set; }

        [Required]
        public double CycleTime { get; set; }

        public int ProductionMethodId { get; set; }

        public int? WorkCentreId { get; set; }

        public int? WorkCentreGroupId { get; set; }

        [InverseProperty("Operations")]
        public ProductionMethod ProductionMethod { get; set; }

        [InverseProperty("AssignedOperations")]
        public WorkCentre WorkCentre { get; set; }

        [InverseProperty("AssignedOperations")]
        public WorkCentreGroup WorkCentreGroup { get; set; }

        public virtual ICollection<Fixture> Fixtures { get; set; }
    }
}