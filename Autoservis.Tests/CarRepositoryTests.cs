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
    public class CarRepositoryTests
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
        public void AddCar_Can()
        {
            var context = GetDbContext();
            var repo = new CarRepository(context);

            var car = new Car { BrandModel = "Škoda Octavia", SPZ = "ABC1234", VIN = "123VIN", Year = 2020 };
            repo.Add(car);

            var result = repo.GetById(car.Id);
            Assert.NotNull(result);
            Assert.Equal("Škoda Octavia", result.BrandModel);
        }

        //Get by ID
        [Fact]
        public void GetById_Null()
        {
            var context = GetDbContext();
            var repo = new CarRepository(context);

            var result = repo.GetById(999);
            Assert.Null(result);
        }

        [Fact]
        public void GetById_NotNull()
        {
            var context = GetDbContext();
            var repo = new CarRepository(context);

            var car = new Car { Id = 999, BrandModel = "Mazda 6", SPZ = "DEF5678", VIN = "456VIN", Year = 2018 };
            repo.Add(car);

            var result = repo.GetById(999);
            Assert.NotNull(result);
            Assert.Equal(car.BrandModel, result.BrandModel);
        }

        // Get all
        [Fact]
        public void GetAll_Null()
        {
            var context = GetDbContext();
            var repo = new CarRepository(context);

            var result = repo.GetAll();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAll_NotNull()
        {
            var context = GetDbContext();
            var repo = new CarRepository(context);

            repo.Add( new Car { Id = 123, BrandModel = "Mazda 6", SPZ = "DEF5678", VIN = "456VIN", Year = 2018 });
            repo.Add( new Car { Id = 456, BrandModel = "Audi a4", SPZ = "ABC123", VIN = "999VIN", Year = 2010 });

            var result = repo.GetAll();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        // Update
        [Fact]
        public void Update()
        {
            var context = GetDbContext();
            var repo = new CarRepository(context);

            var car = new Car { BrandModel = "Old", SPZ = "DEF5678", VIN = "456VIN", Year = 2018 };
            repo.Add(car);

            car.BrandModel = "New";
            car.Year = 2025;
            repo.Update(car);

            var updated = repo.GetById(car.Id);
            Assert.Equal("New", updated.BrandModel);
            Assert.Equal(2025, updated.Year);
        }

        // Delete
        [Fact]
        public void DeleteCar_Exist()
        {
            var context = GetDbContext();
            var repo = new CarRepository(context);

            var car = new Car { BrandModel = "Mazda 6", SPZ = "DEF5678", VIN = "456VIN", Year = 2018 };
            repo.Add(car);
            repo.Delete(car.Id);

            var result = repo.GetById(car.Id);
            Assert.Null(result);
        }

        [Fact]
        public void DeleteCar_DontExist()
        {
            var context = GetDbContext();
            var repo = new CarRepository(context);

            var car = new Car { Id = 456, BrandModel = "Mazda 6", SPZ = "DEF5678", VIN = "456VIN", Year = 2018 };
            repo.Add(car);
            repo.Delete(123);

            var result = repo.GetById(456);
            Assert.NotNull(result);
        }
    }

}
