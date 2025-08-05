using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservis.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public List<Material> Materials { get; set; } = new();
        public List<Work> Works { get; set; } = new();
        public List<Photo> Photos { get; set; } 

        public int TotalPrice => Materials.Sum(m => m.TotalPrice) + Works.Sum(w => w.TotalPrice);

    }
}
