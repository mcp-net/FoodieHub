// SqlPriceRangeRepository.cs
using Microsoft.EntityFrameworkCore;
using FoodieHub.API.Data;
using FoodieHub.API.Models.Domain;

namespace FoodieHub.API.Repositories
{
    public class SqlPriceRangeRepository : IPriceRangeRepository
    {
        private readonly FoodieHubDbContext _db;

        public SqlPriceRangeRepository(FoodieHubDbContext db) =>
            _db = db;

        public async Task<List<PriceRange>> GetAllAsync() =>
            await _db.PriceRanges
                     .AsNoTracking()
                     .OrderBy(pr => pr.Name)
                     .ToListAsync();

        public async Task<PriceRange?> GetByIdAsync(Guid id) =>
            await _db.PriceRanges.FindAsync(id);

        public async Task<PriceRange> CreateAsync(PriceRange pr)
        {
            pr.Id = Guid.NewGuid();
            await _db.PriceRanges.AddAsync(pr);
            await _db.SaveChangesAsync();
            return pr;
        }

        public async Task<PriceRange?> UpdateAsync(Guid id, PriceRange pr)
        {
            var existing = await _db.PriceRanges.FindAsync(id);
            if (existing == null) return null;

            existing.Name = pr.Name;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _db.PriceRanges.FindAsync(id);
            if (existing == null) return false;

            _db.PriceRanges.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
