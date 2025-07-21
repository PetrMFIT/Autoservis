using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autoservis.Models;

namespace Autoservis.Repositories
{
    public interface ICustomerRepository
    {
        void Add(Customer customer);
        Customer? GetById(int id);
        List<Customer> GetAll();
        void Update(Customer customer);
        void Delete(int id);
    }
}
