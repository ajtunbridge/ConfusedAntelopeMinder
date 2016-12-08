#region Using directives

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
    }
}