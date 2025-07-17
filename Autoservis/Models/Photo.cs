using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservis.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
