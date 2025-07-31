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
    public class OrderRepositoryTests
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
        public void AddOrder_Can()
        {
            var context = GetDbContext();
            var repo = new OrderRepository(context);

            var order = new Order { Date = 2025, Name = "Vymena oleje" };
            repo.Add(order);

            var result = repo.GetById(order.Id);
            Assert.NotNull(result);
            Assert.Equal(2025, result.Date);
        }

        //Get by ID
        [Fact]
        public void GetById_Null()
        {
            var context = GetDbContext();
            var repo = new OrderRepository(context);

            var result = repo.GetById(999);
            Assert.Null(result);
        }

        [Fact]
        public void GetById_NotNull()
        {
            var context = GetDbContext();
            var repo = new OrderRepository(context);

            var order = new Order { Id = 999, Date = 2025, Name = "Vymena oleje" };
            repo.Add(order);

            var result = repo.GetById(999);
            Assert.NotNull(result);
            Assert.Equal(order.Date, result.Date);
        }

        // Get all
        [Fact]
        public void GetAll_Null()
        {
            var context = GetDbContext();
            var repo = new OrderRepository(context);

            var result = repo.GetAll();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAll_NotNull()
        {
            var context = GetDbContext();
            var repo = new OrderRepository(context);

            repo.Add(new Order { Id = 123, Date = 2025, Name = "Vymena oleje" });
            repo.Add(new Order { Id = 456, Date = 2024, Name = "Vymena filtru" });

            var result = repo.GetAll();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        // Update
        [Fact]
        public void Update()
        {
            var context = GetDbContext();
            var repo = new OrderRepository(context);

            var order = new Order { Date = 2025, Name = "Vymena oleje" };
            repo.Add(order);

            order.Date = 2024;
            order.Name = "Vymena filtru";
            repo.Update(order);

            var updated = repo.GetById(order.Id);
            Assert.Equal(2024, updated.Date);
            Assert.Equal("Vymena filtru", updated.Name);
        }

        // Delete
        [Fact]
        public void DeleteOrder_Exist()
        {
            var context = GetDbContext();
            var repo = new OrderRepository(context);

            var order = new Order { Date = 2025, Name = "Vymena oleje" };
            repo.Add(order);
            repo.Delete(order.Id);

            var result = repo.GetById(order.Id);
            Assert.Null(result);
        }

        [Fact]
        public void DeleteOrder_DontExist()
        {
            var context = GetDbContext();
            var repo = new OrderRepository(context);

            var order = new Order { Id = 456, Date = 2025, Name = "Vymena oleje" };
            repo.Add(order);
            repo.Delete(123);

            var result = repo.GetById(456);
            Assert.NotNull(result);
        }

        /*** Entity relations ***/

        // Customer
        [Fact]
        public void OrderWithCustomer()
        {
            var context = GetDbContext();

            var customer = new Customer { Name = "Jan Novak" };
            context.Customers.Add(customer);
            context.SaveChanges();

            var order = new Order { Name = "zakazka", CustomerId = customer.Id };
            context.Orders.Add(order);
            context.SaveChanges();

            var result = context.Orders.Include(o => o.Customer).FirstOrDefault(c => c.Id == customer.Id);
            Assert.NotNull(result);
            Assert.NotNull(result.Customer);
            Assert.Equal("Jan Novak", result.Customer.Name);
        }

        // Material
        [Fact]
        public void OrderWithMaterials()
        {
            var context = GetDbContext();

            var order = new Order { Name = "zakazka" };
            context.Orders.Add(order);
            context.SaveChanges();

            var material1 = new Material { Code = "abc123", OrderId = order.Id };
            var material2 = new Material { Code = "def456", OrderId = order.Id };
            context.Materials.AddRange(material1, material2);
            context.SaveChanges();

            var result = context.Orders.Include(m => m.Materials).FirstOrDefault(o => o.Id == order.Id);

            Assert.NotNull(result);
            Assert.NotNull(result.Materials);
            Assert.Equal(2, result.Materials.Count);
            Assert.Contains(result.Materials, m => m.Code == "abc123");
            Assert.Contains(result.Materials, m => m.Code == "def456");
        }

        [Fact]
        public void CascadeDeleteMaterial()
        {
            var context = GetDbContext();

            var order = new Order { Name = "zakazka" };
            context.Orders.Add(order);
            context.SaveChanges();

            var material = new Material { Code = "abc123", OrderId = order.Id };
            context.Materials.Add(material);
            context.SaveChanges();

            context.Orders.Remove(order);
            context.SaveChanges();

            var result = context.Materials.FirstOrDefault(o => o.Id == order.Id);
            Assert.Null(result);
        }

        // Work
        [Fact]
        public void OrderWithWorks()
        {
            var context = GetDbContext();

            var order = new Order { Name = "zakazka" };
            context.Orders.Add(order);
            context.SaveChanges();

            var work1 = new Work { Hours = 20, OrderId = order.Id };
            var work2 = new Work { Hours = 15, OrderId = order.Id };
            context.Works.AddRange(work1, work2);
            context.SaveChanges();

            var result = context.Orders.Include(m => m.Works).FirstOrDefault(o => o.Id == order.Id);

            Assert.NotNull(result);
            Assert.NotNull(result.Works);
            Assert.Equal(2, result.Works.Count);
            Assert.Contains(result.Works, m => m.Hours == 20);
            Assert.Contains(result.Works, m => m.Hours == 15);
        }

        [Fact]
        public void CascadeDeleteWork()
        {
            var context = GetDbContext();

            var order = new Order { Name = "zakazka" };
            context.Orders.Add(order);
            context.SaveChanges();

            var work = new Work { Hours = 20, OrderId = order.Id };
            context.Works.Add(work);
            context.SaveChanges();

            context.Orders.Remove(order);
            context.SaveChanges();

            var result = context.Works.FirstOrDefault(o => o.Id == order.Id);
            Assert.Null(result);
        }

    }
}
