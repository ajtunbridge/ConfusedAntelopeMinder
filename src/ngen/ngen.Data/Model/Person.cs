#region Using directives

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace ngen.Data.Model
{
    [Table("People")]
    public class Person
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [NotMapped]
        public string PhotoAddress => $"Person:{Id}";
    }
}