using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservis.Enums
{
    public enum CarType
    {
        Osobní,
        Nákladní,
        Užitkové,
        Motorka
    }
    public enum FuelType
    {
        Benzín,
        Nafta
    }
    public enum ViewType
    {
        Customers,
        Cars,
        Orders
    }

    public enum MeasureUnit
    {
        ks,
        l
    }

    public enum State
    {
        Rozpracováno,
        Hotovo,
        Zaplaceno
    }

    public enum MaterialSupplier
    {
        LKQ,
        JM_Autodíly,
        Slavíček,
        Cora,
        Intercars
    }
}
