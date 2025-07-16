using FoodieHub.API.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FoodieHub.API.Data
{
    public class FoodieHubDbContext : DbContext
    {
        public FoodieHubDbContext(DbContextOptions<FoodieHubDbContext> options) : base(options)
        {
        }

        public DbSet<PriceRange> PriceRanges { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for PriceRanges
            var priceRanges = new List<PriceRange>()
            {
                new PriceRange()
                {
                    Id = Guid.Parse("54466f17-02af-48e7-8ed3-5a4a8bfacf6f"),
                    Name = "Budget-Friendly"
                },
                new PriceRange()
                {
                    Id = Guid.Parse("ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c"),
                    Name = "Moderate"
                },
                new PriceRange()
                {
                    Id = Guid.Parse("f808ddcd-b5e5-4d80-b732-1ca523e48434"),
                    Name = "Fine Dining"
                }
            };

            // Seed price ranges to the database
            modelBuilder.Entity<PriceRange>().HasData(priceRanges);

            // Seed data for Cities
            var cities = new List<City>
            {
                new City
                {
                    Id = Guid.Parse("f7248fc3-2585-4efb-8d1d-1c555f4087f6"),
                    Name = "New York",
                    Code = "NYC",
                    CityImageUrl = "https://images.pexels.com/photos/2190283/pexels-photo-2190283.jpeg"
                },
                new City
                {
                    Id = Guid.Parse("6884f7d7-ad1f-4101-8df3-7a6fa7387d81"),
                    Name = "Los Angeles",
                    Code = "LA",
                    CityImageUrl = null
                },
                new City
                {
                    Id = Guid.Parse("14ceba71-4b51-4777-9b17-46602cf66153"),
                    Name = "Chicago",
                    Code = "CHI",
                    CityImageUrl = null
                },
                new City
                {
                    Id = Guid.Parse("cfa06ed2-bf65-4b65-93ed-c9d286ddb0de"),
                    Name = "San Francisco",
                    Code = "SF",
                    CityImageUrl = "https://images.pexels.com/photos/1259279/pexels-photo-1259279.jpeg"
                },
                new City
                {
                    Id = Guid.Parse("906cb139-415a-4bbb-a174-1a1faf9fb1f6"),
                    Name = "Miami",
                    Code = "MIA",
                    CityImageUrl = "https://images.pexels.com/photos/2422461/pexels-photo-2422461.jpeg"
                },
                new City
                {
                    Id = Guid.Parse("f077a22e-4248-4bf6-b564-c7cf4e250263"),
                    Name = "Boston",
                    Code = "BOS",
                    CityImageUrl = null
                }
            };

            modelBuilder.Entity<City>().HasData(cities);
        }
    }
}