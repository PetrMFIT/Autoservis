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
    public class WorkRepositoryTests
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
        public void AddWork_Can()
        {
            var context = GetDbContext();
            var repo = new WorkRepository(context);

            var work = new Work { Hours = 10, Price = 450 };
            repo.Add(work);

            var result = repo.GetById(work.Id);
            Assert.NotNull(result);
            Assert.Equal(10, result.Hours);
        }

        //Get by ID
        [Fact]
        public void GetById_Null()
        {
            var context = GetDbContext();
            var repo = new WorkRepository(context);

            var result = repo.GetById(999);
            Assert.Null(result);
        }

        [Fact]
        public void GetById_NotNull()
        {
            var context = GetDbContext();
            var repo = new WorkRepository(context);

            var work = new Work { Hours = 10, Price = 450 };
            repo.Add(work);

            var result = repo.GetById(work.Id);
            Assert.NotNull(result);
            Assert.Equal(work.Hours, result.Hours);
        }

        // Get all
        [Fact]
        public void GetAll_Null()
        {
            var context = GetDbContext();
            var repo = new WorkRepository(context);

            var result = repo.GetAll();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAll_NotNull()
        {
            var context = GetDbContext();
            var repo = new WorkRepository(context);

            repo.Add(new Work { Id = 123, Hours = 10, Price = 450 });
            repo.Add(new Work { Id = 456, Hours = 15, Price = 450 });

            var result = repo.GetAll();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        // Update
        [Fact]
        public void Update()
        {
            var context = GetDbContext();
            var repo = new WorkRepository(context);

            var work = new Work { Hours = 10, Price = 450 };
            repo.Add(work);

            work.Hours = 12;
            work.Price = 400;
            repo.Update(work);

            var updated = repo.GetById(work.Id);
            Assert.Equal(12, updated.Hours);
            Assert.Equal(400, updated.Price);
        }

        // Delete
        [Fact]
        public void DeleteWork_Exist()
        {
            var context = GetDbContext();
            var repo = new WorkRepository(context);

            var work = new Work { Hours = 10, Price = 450 };
            repo.Add(work);
            repo.Delete(work.Id);

            var result = repo.GetById(work.Id);
            Assert.Null(result);
        }

        [Fact]
        public void DeleteWork_DontExist()
        {
            var context = GetDbContext();
            var repo = new WorkRepository(context);

            var work = new Work { Id = 456, Hours = 10, Price = 450 };
            repo.Add(work);
            repo.Delete(123);

            var result = repo.GetById(456);
            Assert.NotNull(result);
        }
    }

}
