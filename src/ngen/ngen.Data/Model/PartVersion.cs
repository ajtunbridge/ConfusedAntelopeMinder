#region Using directives

using System.ComponentModel.DataAnnotations;

#endregion

namespace ngen.Data.Model
{
    public class PartVersion
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(12)]
        public string VersionNumber { get; set; }

        public string Changes { get; set; }

        public int PartId { get; set; }

        public Part Part { get; set; }
    }
}