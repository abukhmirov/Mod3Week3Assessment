using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Tourism.DataAccess;
using Tourism.Models;


namespace Tourism.FeatureTests
{
    public class StatesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public StatesControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private TourismContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TourismContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");

            var context = new TourismContext(optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }

        [Fact]
        public async Task New_ReturnsFormView()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/states/new");
            var html = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("Add a State", html);
            Assert.Contains("<form method=\"post\" action=\"/states\">", html);
        }

        [Fact]
        public async Task AddState_ReturnsRedirectToShow()
        {
            
            var formData = new Dictionary<string, string>
    {
        { "Name", "Colorado" },
        { "Abbreviation", "CO" }
    };

            var client = _factory.CreateClient();

           
            var response = await client.PostAsync("/states", new FormUrlEncodedContent(formData));
            var html = await response.Content.ReadAsStringAsync();

            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            Assert.Contains("Colorado", html);
            

            // Assert that the movie was added to the database. This test isn't mandatory, but testing against what's in the database is a useful testing tool to add to your toolbox.
            var context = GetDbContext();
            var savedState = await context.States.FirstOrDefaultAsync(
                m => m.Name == "Colorado"
            );
            Assert.NotNull(savedState);
            Assert.Equal("Colorado", savedState.Name);
        }
    }
}