using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngen.Data.Model
{
    public class Fixture
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Caption { get; set; }

        [MaxLength(-1)]
        public string Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string Location { get; set; }
        
        public int PartId { get; set; }

        public Part Part { get; set; }

        [NotMapped]
        public string PhotoAddress => $"Fixture:{Id}";
    }
}
