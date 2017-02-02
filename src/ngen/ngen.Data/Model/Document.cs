#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
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

        [NotMapped]
        public bool IsCheckedOut => CheckedOutById.HasValue;

        [MaxLength(-1)]
        public byte[] Permissions { get; set; }

        public int? DocumentFolderId { get; set; }
        
        public int? PartId { get; set; }

        public int? PartVersionId { get; set; }

        public int? CheckedOutById { get; set; }

        public DateTime? CheckedOutAt { get; set; }
        
        public virtual DocumentFolder DocumentFolder { get; set; }
        
        public virtual Part Part { get; set; }

        public virtual PartVersion PartVersion { get; set; }

        public virtual Employee CheckedOutBy { get; set; }

        public virtual ICollection<DocumentVersion> DocumentVersions { get; set; }
    }
}