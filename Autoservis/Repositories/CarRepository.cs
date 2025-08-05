using Autoservis.Data;
using Autoservis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Autoservis.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly AppDbContext _context;

        public CarRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Car car)
        {
            _context.Cars.Add(car);
            _context.SaveChanges();
        }

        public Car? GetById(int id)
        {
            return _context.Cars.Find(id);
        }

        public IEnumerable<Car> GetAll()
        {
            return _context.Cars.Include(c => c.Customer).ToList();
        }

        public void Update(Car car)
        {
            _context.Cars.Update(car);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var car = _context.Cars.Find(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                _context.SaveChanges();
            }
        }
    }
}
