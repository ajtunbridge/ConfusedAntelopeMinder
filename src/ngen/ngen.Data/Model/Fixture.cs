using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ngen.Data.Model
{
    public class Fixture
    {
        public int Id { get; set; }

        /// <summary>
        /// The display name for this fixture
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Caption { get; set; }

        /// <summary>
        /// A description of the fixture and it's usage
        /// </summary>
        [MaxLength(-1)]
        public string Description { get; set; }

        /// <summary>
        /// The location of this fixture
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Location { get; set; }

        public int? OperationId { get; set; }

        /// <summary>
        /// The operation this fixture is used on
        /// </summary>
        [InverseProperty("Fixtures")]
        public Operation Operation { get; set; }

        [NotMapped]
        public string PhotoAddress => $"Fixture:{Id}";
    }
}