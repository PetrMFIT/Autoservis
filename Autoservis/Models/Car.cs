using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autoservis.Enums;

namespace Autoservis.Models
{
    public class Car
    {
        public int Id { get; set; }
        [Required]
        public string BrandModel { get; set; } = string.Empty;
        [Required]
        public string SPZ { get; set; } = string.Empty;
        [Required]
        public string VIN { get; set; } = string.Empty;
        public int Year { get; set; }
        public FuelType? Fuel { get; set; }
        public CarType? Type {  get; set; } 
        public string DisplacementPower { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public List<Order> Orders { get; set; } = new();
    }
}
