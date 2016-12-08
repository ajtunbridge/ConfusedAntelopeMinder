#region Using directives

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

        public Customer Customer { get; set; }
        
        public int? Primary2dDrawingDocumentId { get; set; }

        public int? Primary3dDrawingDocumentId { get; set; }

        public Document Primary2dDrawingDocument { get; set; }

        public Document Primary3dDrawingDocument { get; set; }
    }
}