using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngen.Data.Model
{
    public class Photo
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Address { get; set; }
        
        [MaxLength(50)]
        public string Caption { get; set; }

        [MaxLength(-1)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string FileName { get; set; }

        [Required]
        public bool IsPrimary { get; set; }
    }
}
