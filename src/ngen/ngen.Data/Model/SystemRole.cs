#region Using directives

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#endregion

namespace ngen.Data.Model
{
    public class SystemRole
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public string Description { get; set; }

        [MaxLength(-1)]
        public byte[] Permissions { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}