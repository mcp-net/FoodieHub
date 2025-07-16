//using FoodieHub.API.Models.Domain;

//namespace FoodieHub.API.Repositories
//{
//    public interface ICityRepository
//    {
//        Task<List<City>> GetAllAsync();
//        Task<City?> GetByIdAsync(Guid id);
//        Task<City> CreateAsync(City city);
//        Task<City?> UpdateAsync(Guid id, City city);
//        Task<City?> DeleteAsync(Guid id);
//    }
//}


// Repositories/ICityRepository.cs
using FoodieHub.API.Models.Domain;
using Microsoft.AspNetCore.Http;

public interface ICityRepository
{
    Task<List<City>> GetAllAsync();
    Task<City?> GetByIdAsync(Guid id);
    Task<City> CreateAsync(City city);
    Task<City?> UpdateAsync(Guid id, City city);
    Task<City?> DeleteAsync(Guid id);

    // New methods:
    Task<(List<City> Items, int Total)> GetPagedAsync(
        string? search, int page, int pageSize);

    Task<City?> GetByCodeAsync(string code);

    Task<string> SaveImageAsync(Guid id, IFormFile file);

    Task<int> CountAsync();
}
