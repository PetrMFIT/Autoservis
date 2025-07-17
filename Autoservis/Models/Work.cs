using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservis.Models
{
    public class Work
    {
        public int Id { get; set; }
        public int Hours { get; set; }
        public int Price { get; set; }
        public int TotalPrice => Hours * Price;

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
