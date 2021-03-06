﻿#region Using directives

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace ngen.Data.Model
{
    public class Part
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        public string DrawingNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        public int CustomerId { get; set; }

        public int? Primary2dDrawingDocumentId { get; set; }

        public int? Primary3dDrawingDocumentId { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Document Primary2dDrawingDocument { get; set; }

        public virtual Document Primary3dDrawingDocument { get; set; }

        [InverseProperty("Part")]
        public virtual ICollection<Document> Documents { get; set; }
    }
}