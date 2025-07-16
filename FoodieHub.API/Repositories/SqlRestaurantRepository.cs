using Microsoft.EntityFrameworkCore;
using FoodieHub.API.Data;
using FoodieHub.API.Models.Domain;

namespace FoodieHub.API.Repositories
{
    public class SqlRestaurantRepository : IRestaurantRepository
    {
        private readonly FoodieHubDbContext _dbContext;

        public SqlRestaurantRepository(FoodieHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Restaurant>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            //var restaurants = _dbContext.Restaurants
            //                    .Include("PriceRange")
            //                    .Include("City")
            //                    .AsQueryable();

            // Instead of string-based Include, use lambdas:
            var restaurants = _dbContext.Restaurants
                .Include(r => r.PriceRange)
                .Include(r => r.City)
                .AsNoTracking()      // improves read performance when you don't update
                .AsQueryable();



            // Filtering
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    restaurants = restaurants.Where(x => x.Name.Contains(filterQuery));
                }
            }

            // Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    restaurants = isAscending ? restaurants.OrderBy(x => x.Name) : restaurants.OrderByDescending(x => x.Name);
                }
                else if (sortBy.Equals("Rating", StringComparison.OrdinalIgnoreCase))
                {
                    restaurants = isAscending ? restaurants.OrderBy(x => x.Rating) : restaurants.OrderByDescending(x => x.Rating);
                }
            }

            // Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await restaurants.Skip(skipResults).Take(pageSize).ToListAsync();
        }



        public async Task<Restaurant?> GetByIdAsync(Guid id)
        {
            //return await _dbContext.Restaurants
            //    .Include("PriceRange")
            //    .Include("City")
            //    .FirstOrDefaultAsync(x => x.Id == id);

            // Instead of string-based Include, use lambdas:
            var query = _dbContext.Restaurants
                .Include(r => r.PriceRange)
                .Include(r => r.City)
                .AsNoTracking()      // improves read performance when you don't update
                .AsQueryable();

            return await query.FirstOrDefaultAsync(x => x.Id == id);


        }

        public async Task<Restaurant> CreateAsync(Restaurant restaurant)
        {
            await _dbContext.Restaurants.AddAsync(restaurant);
            await _dbContext.SaveChangesAsync();
            return restaurant;
        }

        public async Task<Restaurant?> UpdateAsync(Guid id, Restaurant restaurant)
        {
            var existingRestaurant = await _dbContext.Restaurants.FirstOrDefaultAsync(x => x.Id == id);

            if (existingRestaurant == null)
            {
                return null;
            }

            existingRestaurant.Name = restaurant.Name;
            existingRestaurant.Description = restaurant.Description;
            existingRestaurant.Address = restaurant.Address;
            existingRestaurant.RestaurantImageUrl = restaurant.RestaurantImageUrl;
            existingRestaurant.Rating = restaurant.Rating;
            existingRestaurant.PriceRangeId = restaurant.PriceRangeId;
            existingRestaurant.CityId = restaurant.CityId;

            await _dbContext.SaveChangesAsync();
            return existingRestaurant;
        }

        public async Task<Restaurant?> DeleteAsync(Guid id)
        {
            var existingRestaurant = await _dbContext.Restaurants.FirstOrDefaultAsync(x => x.Id == id);

            if (existingRestaurant == null)
            {
                return null;
            }

            _dbContext.Restaurants.Remove(existingRestaurant);
            await _dbContext.SaveChangesAsync();
            return existingRestaurant;
        }
    }
}