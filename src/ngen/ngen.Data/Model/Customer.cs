#region Using directives

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace ngen.Data.Model
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string FullName { get; set; }
        
        [MaxLength(40)]
        [Index(IsUnique = true)]
        public string ShortName { get; set; }

        public virtual ICollection<Part> Parts { get; set; } 
    }
}