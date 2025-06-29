using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using wacdoprojet.Controllers;
using wacdoprojet.Data;
using wacdoprojet.Models;
using System.Linq;

namespace wacdoprojet.Tests
{
    public class RestaurantsControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "RestaurantsTestDb")
                .Options;

            var context = new ApplicationDbContext(options);

            // Seed data if needed
            if (!context.Restaurants.Any())
            {
                context.Restaurants.AddRange(
                    new Restaurant { Id = 1, name = "Chez Luigi", adresse = "Paris" },
                    new Restaurant { Id = 2, name = "Bistro 34", adresse = "Lyon" }
                );
                context.SaveChanges();
            }

            return context;
        }

        [Fact]
        public async Task Index_ReturnsFilteredRestaurants()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new RestaurantsController(context);

            // Act
            var result = await controller.Index("Luigi", null) as ViewResult;
            var model = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Restaurant>>(result.Model);

            // Assert
            Assert.Single(model); // 1 seul restaurant avec "Luigi"
            Assert.Contains(model, r => r.name.Contains("Luigi"));
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            var context = GetDbContext();
            var controller = new RestaurantsController(context);

            var result = await controller.Details(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_AddsRestaurant_AndRedirects()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new RestaurantsController(context);
            var newRestaurant = new Restaurant
            {
                name = "Test Resto",
                adresse = "Nice",
                cp = "06000",
                ville = "Nice"
            };

            // Act
            var result = await controller.Create(newRestaurant);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Contains(context.Restaurants, r => r.name == "Test Resto");
        }
    }
}
