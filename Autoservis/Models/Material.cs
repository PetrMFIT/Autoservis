using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autoservis.Enums;

namespace Autoservis.Models
{
    public class Material
    {
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Price { get; set; }
        public MeasureUnit Unit {  get; set; }
        public int TotalPrice => Quantity * Price;

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }

}
