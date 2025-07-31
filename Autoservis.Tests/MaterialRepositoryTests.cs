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
	public class MaterialRepositoryTests
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
		public void AddMaterial_Can()
		{
			var context = GetDbContext();
			var repo = new MaterialRepository(context);

			var material = new Material { Code = "ABC123", Name = "Tesneni", Quantity = 2 };
			repo.Add(material);

			var result = repo.GetById(material.Id);
			Assert.NotNull(result);
			Assert.Equal("ABC123", result.Code);
		}

        //Get by ID
        [Fact]
        public void GetById_Null()
        {
            var context = GetDbContext();
            var repo = new MaterialRepository(context);

            var result = repo.GetById(999);
            Assert.Null(result);
        }

        [Fact]
        public void GetById_NotNull()
        {
            var context = GetDbContext();
            var repo = new MaterialRepository(context);

            var material = new Material { Code = "ABC123", Name = "Tesneni", Quantity = 2 };
            repo.Add(material);

            var result = repo.GetById(material.Id);
            Assert.NotNull(result);
            Assert.Equal(material.Code, result.Code);
        }

        // Get all
        [Fact]
        public void GetAll_Null()
        {
            var context = GetDbContext();
            var repo = new MaterialRepository(context);

            var result = repo.GetAll();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAll_NotNull()
        {
            var context = GetDbContext();
            var repo = new MaterialRepository(context);

            repo.Add(new Material { Id = 123, Code = "ABC123", Name = "Tesneni", Quantity = 2 });
            repo.Add(new Material { Id = 456, Code = "DEF456", Name = "Svicka", Quantity = 1 });

            var result = repo.GetAll();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        // Update
        [Fact]
        public void Update()
        {
            var context = GetDbContext();
            var repo = new MaterialRepository(context);

            var material = new Material { Code = "ABC123", Name = "Tesneni", Quantity = 2 };
            repo.Add(material);

            material.Code = "New";
            material.Quantity = 3;
            repo.Update(material);

            var updated = repo.GetById(material.Id);
            Assert.Equal("New", updated.Code);
            Assert.Equal(3, updated.Quantity);
        }

        // Delete
        [Fact]
        public void DeleteMaterial_Exist()
        {
            var context = GetDbContext();
            var repo = new MaterialRepository(context);

            var material = new Material { Code = "ABC123", Name = "Tesneni", Quantity = 2 };
            repo.Add(material);
            repo.Delete(material.Id);

            var result = repo.GetById(material.Id);
            Assert.Null(result);
        }

        [Fact]
        public void DeleteMaterial_DontExist()
        {
            var context = GetDbContext();
            var repo = new MaterialRepository(context);

            var material = new Material { Code = "ABC123", Name = "Tesneni", Quantity = 2 };
            repo.Add(material);
            repo.Delete(123);

            var result = repo.GetById(material.Id);
            Assert.NotNull(result);
        }

        /*** Entity relations ***/

        // Order
        [Fact]
        public void MaterialWithOrder()
        {
            var context = GetDbContext();

            var order = new Order { Name = "zakazka" };
            context.Orders.Add(order);
            context.SaveChanges();

            var material = new Material { Code = "abc123", OrderId = order.Id };
            context.Materials.Add(material);
            context.SaveChanges();

            var result = context.Materials.Include(m => m.Order).FirstOrDefault(m => m.Id == order.Id);

            Assert.NotNull(result);
            Assert.NotNull(result.Order);
            Assert.Equal("zakazka", result.Order.Name);
        }
    }
}
