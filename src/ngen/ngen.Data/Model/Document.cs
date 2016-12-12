#region Using directives

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [MaxLength(-1)]
        public byte[] Permissions { get; set; }

        public int? DocumentFolderId { get; set; }
        
        public int? PartId { get; set; }

        public int? PartVersionId { get; set; }

        public int? CheckedOutById { get; set; }

        public DateTime? CheckedOutAt { get; set; }

        public DocumentFolder DocumentFolder { get; set; }

        public Part Part { get; set; }

        public PartVersion PartVersion { get; set; }

        public Employee CheckedOutBy { get; set; }
    }
}