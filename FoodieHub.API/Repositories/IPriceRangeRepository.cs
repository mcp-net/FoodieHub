// IPriceRangeRepository.cs
using FoodieHub.API.Models.Domain;

namespace FoodieHub.API.Repositories
{
    public interface IPriceRangeRepository
    {
        Task<List<PriceRange>> GetAllAsync();
        Task<PriceRange?> GetByIdAsync(Guid id);
        Task<PriceRange> CreateAsync(PriceRange priceRange);
        Task<PriceRange?> UpdateAsync(Guid id, PriceRange priceRange);
        Task<bool> DeleteAsync(Guid id);
    }
}
