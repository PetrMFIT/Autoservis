using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autoservis.Models;
using Autoservis.Data;

namespace Autoservis.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Customer customer)
        {
            _context.Add(customer);
            _context.SaveChanges();
        }

        public Customer? GetById(int id)
        {
            return _context.Customers.Find(id);
        }

        public List<Customer> GetAll()
        {
            return _context.Customers.ToList();
        }

        public void Update(Customer customer)
        {
            _context.Update(customer);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }
        }
    }
}
