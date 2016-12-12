#region Using directives

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace ngen.Data.Model
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        [Index(IsUnique = true)]
        public string UserName { get; set; }

        [Required]
        [StringLength(60)]
        [Column(TypeName="nchar")]
        public string Password { get; set; }
        
        [Required]
        public bool IsActive { get; set; }

        public int PersonId { get; set; }

        public int SystemRoleId { get; set; }

        public Person Person { get; set; }

        public SystemRole SystemRole { get; set; }
    }
}