using Autoservis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservis.Helpers
{
    public static class DictioanaryHelper
    {
        public static readonly Dictionary<CarType, string> CarTypeDisplayNames = new()
        {
            { CarType.Car, "Osobní" },
            { CarType.Truck, "Nákladní" },
            { CarType.Van, "Užitková" },
            { CarType.Motorbike, "Motocykl" }
        };

        public static string GetCarTypeDisplayName(CarType carType) =>
            CarTypeDisplayNames.TryGetValue(carType, out var name) ? name : carType.ToString();

        public static readonly Dictionary<FuelType, string> FuelTypeDisplayNames = new()
        {
            { FuelType.Gasoline, "Benzín" },
            { FuelType.Diesel, "Nafta" }
        };

        public static string GetFuelTypeDisplayName(FuelType fuelType) =>
            FuelTypeDisplayNames.TryGetValue(fuelType, out var name) ? name : fuelType.ToString();
    }
}
