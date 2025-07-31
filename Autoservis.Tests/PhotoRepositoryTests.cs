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
    public class PhotoRepositoryTests
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
        public void AddPhoto_Can()
        {
            var context = GetDbContext();
            var repo = new PhotoRepository(context);

            var photo = new Photo { Name = "zakazka", FilePath = "/zakazka.jpg" };
            repo.Add(photo);

            var result = repo.GetById(photo.Id);
            Assert.NotNull(result);
            Assert.Equal("zakazka", result.Name);
        }

        //Get by ID
        [Fact]
        public void GetById_Null()
        {
            var context = GetDbContext();
            var repo = new PhotoRepository(context);

            var result = repo.GetById(999);
            Assert.Null(result);
        }

        [Fact]
        public void GetById_NotNull()
        {
            var context = GetDbContext();
            var repo = new PhotoRepository(context);

            var photo = new Photo { Name = "zakazka", FilePath = "/zakazka.jpg" };
            repo.Add(photo);

            var result = repo.GetById(photo.Id);
            Assert.NotNull(result);
            Assert.Equal(photo.Name, result.Name);
        }

        // Get all
        [Fact]
        public void GetAll_Null()
        {
            var context = GetDbContext();
            var repo = new PhotoRepository(context);

            var result = repo.GetAll();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAll_NotNull()
        {
            var context = GetDbContext();
            var repo = new PhotoRepository(context);

            repo.Add(new Photo { Id = 123, Name = "zakazka", FilePath = "/zakazka.jpg" });
            repo.Add(new Photo { Id = 456, Name = "zakazka2", FilePath = "/zakazka2.jpg" });

            var result = repo.GetAll();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        // Update
        [Fact]
        public void Update()
        {
            var context = GetDbContext();
            var repo = new PhotoRepository(context);

            var photo = new Photo { Name = "zakazka", FilePath = "/zakazka.jpg" };
            repo.Add(photo);

            photo.Name = "New";
            photo.FilePath = "/new.jpg";
            repo.Update(photo);

            var updated = repo.GetById(photo.Id);
            Assert.Equal("New", updated.Name);
            Assert.Equal("/new.jpg", updated.FilePath);
        }

        // Delete
        [Fact]
        public void DeletePhoto_Exist()
        {
            var context = GetDbContext();
            var repo = new PhotoRepository(context);

            var photo = new Photo { Name = "zakazka", FilePath = "/zakazka.jpg" };
            repo.Add(photo);
            repo.Delete(photo.Id);

            var result = repo.GetById(photo.Id);
            Assert.Null(result);
        }

        [Fact]
        public void DeletePhoto_DontExist()
        {
            var context = GetDbContext();
            var repo = new PhotoRepository(context);

            var photo = new Photo { Id = 456, Name = "zakazka", FilePath = "/zakazka.jpg" };
            repo.Add(photo);
            repo.Delete(123);

            var result = repo.GetById(456);
            Assert.NotNull(result);
        }

        /*** Entity relations ***/

        // Order
        [Fact]
        public void PhotoWithOrder()
        {
            var context = GetDbContext();

            var order = new Order { Name = "zakazka" };
            context.Orders.Add(order);
            context.SaveChanges();

            var photo = new Photo { Name = "photo", OrderId = order.Id };
            context.Photos.Add(photo);
            context.SaveChanges();

            var result = context.Photos.Include(o => o.Order).FirstOrDefault(c => c.Id == photo.Id);

            Assert.NotNull(result);
            Assert.NotNull(result.Order);
            Assert.Equal("zakazka", result.Order.Name);
        }
    }

}
