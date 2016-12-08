#region Using directives

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace ngen.Data.Model
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Name { get; set; }
    }
}