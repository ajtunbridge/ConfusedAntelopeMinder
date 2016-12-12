using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngen.Data.Model
{
    public class ClientSetting
    {
        public int Id { get; set; }

        [Required]
        [Index(IsUnique=true)]
        [MaxLength(100)]
        public string CompanyName { get; set; }

        /// <summary>
        /// Azure, FTP or Local
        /// </summary>
        [Required]
        [MaxLength(5)]
        public string DocumentStorageTechnology { get; set; }
    }
}