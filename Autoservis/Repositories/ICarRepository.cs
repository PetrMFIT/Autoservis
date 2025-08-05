using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autoservis.Models;

namespace Autoservis.Repositories
{
    public interface ICarRepository
    {
        void Add(Car car);
        Car? GetById(int id);
        IEnumerable<Car> GetAll();
        void Update(Car car);
        void Delete(int id);

    }
}
