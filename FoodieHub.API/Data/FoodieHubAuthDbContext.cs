using FoodieHub.API.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FoodieHub.API.Data
{
    public class FoodieHubAuthDbContext : IdentityDbContext
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public FoodieHubAuthDbContext(DbContextOptions<FoodieHubAuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var reviewerRoleId = "54466f17-02af-48e7-8ed3-5a4a8bfacf6f";
            var restaurantOwnerRoleId = "ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = reviewerRoleId,
                    ConcurrencyStamp = reviewerRoleId,
                    Name = "Reviewer",
                    NormalizedName = "REVIEWER"
                },
                new IdentityRole
                {
                    Id = restaurantOwnerRoleId,
                    ConcurrencyStamp = restaurantOwnerRoleId,
                    Name = "RestaurantOwner",
                    NormalizedName = "RESTAURANTOWNER"
                }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}