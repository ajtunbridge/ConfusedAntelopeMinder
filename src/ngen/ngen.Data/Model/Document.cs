#region Using directives

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace ngen.Data.Model
{
    public class Document
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FileName { get; set; }

        public bool IsCheckedOut { get; set; }

        [MaxLength(-1)] // varbinary(MAX)
        public byte[] Permissions { get; set; }

        public DateTime? CheckedOutAt { get; set; }

        public int? CheckedOutById { get; set; }

        public Employee CheckedOutBy { get; set; }
    }
}