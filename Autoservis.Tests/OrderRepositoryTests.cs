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
    }

}
