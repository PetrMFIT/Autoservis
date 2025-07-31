using Autoservis.Data;
using Autoservis.Models;
using Autoservis.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservis.Tests
{
    public class CustomerRepositoryTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        // Add
        [Fact]
        public void AddCustomer_Can()
        {
            var context = GetDbContext();
            var repo = new CustomerRepository(context);

            var customer = new Customer { Name = "Jan Novak", Phone = "123456789", Email = "novak@email.com" };
            repo.Add(customer);

            var result = repo.GetById(customer.Id);
            Assert.NotNull(result);
            Assert.Equal("Jan Novak", result.Name);
        }
        
        //Get by ID
        [Fact]
        public void GetById_Null()
        {
            var context = GetDbContext();
            var repo = new CustomerRepository(context);

            var result = repo.GetById(999);
            Assert.Null(result);
        }
        
        [Fact]
        public void GetById_NotNull()
        {
            var context = GetDbContext();
            var repo = new CustomerRepository(context);

            var customer = new Customer { Id = 999, Name = "Jan Novak", Phone = "123456789", Email = "novak@email.com" };
            repo.Add(customer);

            var result = repo.GetById(999);
            Assert.NotNull(result);
            Assert.Equal(customer.Name, result.Name);
        }
        
        // Get all
        [Fact]
        public void GetAll_Null()
        {
            var context = GetDbContext();
            var repo = new CustomerRepository(context);

            var result = repo.GetAll();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAll_NotNull()
        {
            var context = GetDbContext();
            var repo = new CustomerRepository(context);

            repo.Add(new Customer { Id = 123, Name = "Jan Novak", Phone = "123456789", Email = "novak@email.com" });
            repo.Add(new Customer { Id = 456, Name = "Josef Novak", Phone = "987654321", Email = "novak2@email.com" });

            var result = repo.GetAll();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        // Update
        [Fact]
        public void Update()
        {
            var context = GetDbContext();
            var repo = new CustomerRepository(context);

            var customer = new Customer { Name = "Jan Novak", Phone = "123456789", Email = "novak@email.com" };
            repo.Add(customer);

            customer.Name = "Josef";
            customer.Email = "novak2@email.com";
            repo.Update(customer);

            var updated = repo.GetById(customer.Id);
            Assert.Equal("Josef", updated.Name);
            Assert.Equal("novak2@email.com", updated.Email);
        }

        // Delete
        [Fact]
        public void DeleteCustomer_Exist()
        {
            var context = GetDbContext();
            var repo = new CustomerRepository(context);

            var customer = new Customer { Name = "Jan Novak", Phone = "123456789", Email = "novak@email.com" };
            repo.Add(customer);
            repo.Delete(customer.Id);

            var result = repo.GetById(customer.Id);
            Assert.Null(result);
        }

        [Fact]
        public void DeleteCustomer_DontExist()
        {
            var context = GetDbContext();
            var repo = new CustomerRepository(context);

            var customer = new Customer { Id = 456, Name = "Jan Novak", Phone = "123456789", Email = "novak@email.com" };
            repo.Add(customer);
            repo.Delete(123);

            var result = repo.GetById(456);
            Assert.NotNull(result);
        }

        /*** Entity relations ***/

        // Cars
        [Fact]
        public void CustomerWithCars()
        {
            var context = GetDbContext();
            var repo = new CustomerRepository(context);

            var customer = new Customer { Name = "Jan Novak", Phone = "123456789", Email = "novak@email.com" };
            repo.Add(customer);

            var car1 = new Car { BrandModel = "Skoda Octavia", SPZ = "123ABC", VIN = "VIN123", CustomerId = customer.Id };
            var car2 = new Car { BrandModel = "Ford Focus", SPZ = "456DEF", VIN = "VIN456", CustomerId = customer.Id };
            context.Cars.AddRange(car1, car2);
            context.SaveChanges();

            var result = context.Customers.Include(c => c.Cars).FirstOrDefault(c => c.Id == customer.Id);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Cars);
            Assert.Equal(2,result.Cars.Count());
            Assert.Contains(result.Cars, c => c.BrandModel == "Skoda Octavia");
            Assert.Contains(result.Cars, c => c.BrandModel == "Ford Focus");
        }

        [Fact]
        public void CascadeDeleteCars()
        {
            var context = GetDbContext();

            var customer = new Customer { Name = "Jan Novak" };
            context.Customers.Add(customer);
            context.SaveChanges();

            var car = new Car { BrandModel = "Toyota", SPZ = "ABC123", VIN = "VIN001", CustomerId = customer.Id };
            context.Cars.Add(car);
            context.SaveChanges();

            context.Customers.Remove(customer);
            context.SaveChanges();

            var result = context.Cars.FirstOrDefault(c => c.Id == car.Id);
            Assert.Null(result);
        }

        // Orders
        [Fact]
        public void CustomerWithOrders()
        {
            var context = GetDbContext();
            var repo = new CustomerRepository(context);

            var customer = new Customer { Name = "Jan Novak", Phone = "123456789", Email = "novak@email.com" };
            repo.Add(customer);

            var order1 = new Order { Date = 2025, Name = "zakazka1", CustomerId = customer.Id };
            var order2 = new Order { Date = 2024, Name = "zakazka2", CustomerId = customer.Id };
            context.Orders.AddRange(order1, order2);
            context.SaveChanges();

            var result = context.Customers.Include(o => o.Orders).FirstOrDefault(c => c.Id == customer.Id);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Orders);
            Assert.Equal(2, result.Orders.Count());
            Assert.Contains(result.Orders, o => o.Name == "zakazka1");
            Assert.Contains(result.Orders, o => o.Name == "zakazka2");

        }

        [Fact]
        public void CascadeDeleteOrder()
        {
            var context = GetDbContext();

            var customer = new Customer { Name = "Jan Novak" };
            context.Customers.Add(customer);
            context.SaveChanges();

            var order = new Order { Name = "zakazka", CustomerId = customer.Id };
            context.Orders.Add(order);
            context.SaveChanges();

            context.Customers.Remove(customer);
            context.SaveChanges();

            var result = context.Orders.FirstOrDefault(o => o.Id == order.Id);
            Assert.Null(result);
        }
    }

}
