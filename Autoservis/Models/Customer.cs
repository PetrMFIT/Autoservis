using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservis.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ZIP { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public List<Car> Cars { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
    }
}
