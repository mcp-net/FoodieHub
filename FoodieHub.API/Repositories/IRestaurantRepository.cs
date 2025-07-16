using FoodieHub.API.Models.Domain;

namespace FoodieHub.API.Repositories
{
    public interface IRestaurantRepository
    {
        Task<List<Restaurant>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        Task<Restaurant?> GetByIdAsync(Guid id);
        Task<Restaurant> CreateAsync(Restaurant restaurant);
        Task<Restaurant?> UpdateAsync(Guid id, Restaurant restaurant);
        Task<Restaurant?> DeleteAsync(Guid id);
    }
}