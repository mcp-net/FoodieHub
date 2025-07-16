// Repositories/SqlCityRepository.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using FoodieHub.API.Data;
using FoodieHub.API.Models.Domain;

namespace FoodieHub.API.Repositories
{
    public class SqlCityRepository : ICityRepository
    {
        private readonly FoodieHubDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _http;

        public SqlCityRepository(
            FoodieHubDbContext db,
            IWebHostEnvironment env,
            IHttpContextAccessor http)
        {
            _db = db;
            _env = env;
            _http = http;
        }

        // 1. Core CRUD
        public async Task<List<City>> GetAllAsync() =>
            await _db.Cities.AsNoTracking().ToListAsync();

        public async Task<City?> GetByIdAsync(Guid id) =>
            await _db.Cities.FindAsync(id);

        public async Task<City> CreateAsync(City city)
        {
            city.Id = Guid.NewGuid();
            await _db.Cities.AddAsync(city);
            await _db.SaveChangesAsync();
            return city;
        }

        public async Task<City?> UpdateAsync(Guid id, City city)
        {
            var existing = await _db.Cities.FindAsync(id);
            if (existing == null) return null;

            // copy fields
            existing.Code = city.Code;
            existing.Name = city.Name;
            existing.CityImageUrl = city.CityImageUrl;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<City?> DeleteAsync(Guid id)
        {
            var existing = await _db.Cities.FindAsync(id);
            if (existing == null) return null;

            _db.Cities.Remove(existing);
            await _db.SaveChangesAsync();
            return existing;
        }

        // 2. Paging
        public async Task<(List<City> Items, int Total)> GetPagedAsync(
            string? search, int page, int pageSize)
        {
            var query = _db.Cities.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Name.Contains(search));

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        // 3. Lookup by Code
        public async Task<City?> GetByCodeAsync(string code) =>
            await _db.Cities
                .FirstOrDefaultAsync(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

        // 4. Image Save
        public async Task<string> SaveImageAsync(Guid id, IFormFile file)
        {
            var folder = Path.Combine(_env.WebRootPath, "Images");
            Directory.CreateDirectory(folder);

            var fileName = $"{id}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(folder, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            // return the relative URL
            return $"/Images/{fileName}";
        }

        // 5. Total Count
        public async Task<int> CountAsync() =>
            await _db.Cities.CountAsync();
    }
}










//////////// Repositories/SqlCityRepository.cs
//////////using FoodieHub.API.Data;
//////////using FoodieHub.API.Models.Domain;
//////////using Microsoft.AspNetCore.Http;
//////////using Microsoft.EntityFrameworkCore;
//////////using Microsoft.EntityFrameworkCore.Infrastructure;

//////////public class SqlCityRepository : ICityRepository
//////////{
//////////    private readonly FoodieHubDbContext _db;

//////////    public SqlCityRepository(FoodieHubDbContext db) => _db = db;

//////////    // Existing CRUD methods…

//////////    public async Task<(List<City> Items, int Total)> GetPagedAsync(
//////////        string? search, int page, int pageSize)
//////////    {
//////////        var query = _db.Cities.AsQueryable();
//////////        if (!string.IsNullOrWhiteSpace(search))
//////////            query = query.Where(c => c.Name.Contains(search));

//////////        var total = await query.CountAsync();
//////////        var items = await query
//////////            .Skip((page - 1) * pageSize)
//////////            .Take(pageSize)
//////////            .ToListAsync();

//////////        return (items, total);
//////////    }

//////////    public async Task<City?> GetByCodeAsync(string code) =>
//////////        await _db.Cities.FirstOrDefaultAsync(c =>
//////////            c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

//////////    public async Task<string> SaveImageAsync(Guid id, IFormFile file)
//////////    {
//////////        // Example: save to wwwroot/Images and return URL
//////////        var folder = Path.Combine(_db.GetService<IWebHostEnvironment>()
//////////                         .WebRootPath, "Images");
//////////        Directory.CreateDirectory(folder);

//////////        var fileName = $"{id}_{Path.GetFileName(file.FileName)}";
//////////        var fullPath = Path.Combine(folder, fileName);

//////////        await using var stream = new FileStream(fullPath, FileMode.Create);
//////////        await file.CopyToAsync(stream);

//////////        return $"/Images/{fileName}";
//////////    }

//////////    public async Task<int> CountAsync() =>
//////////        await _db.Cities.CountAsync();
//////////}






////////////using Microsoft.EntityFrameworkCore;
////////////using FoodieHub.API.Data;
////////////using FoodieHub.API.Models.Domain;

////////////namespace FoodieHub.API.Repositories
////////////{
////////////    public class SqlCityRepository : ICityRepository
////////////    {
////////////        private readonly FoodieHubDbContext _dbContext;

////////////        public SqlCityRepository(FoodieHubDbContext dbContext)
////////////        {
////////////            _dbContext = dbContext;
////////////        }

////////////        public async Task<List<City>> GetAllAsync()
////////////        {
////////////            return await _dbContext.Cities.ToListAsync();
////////////        }

////////////        public async Task<City?> GetByIdAsync(Guid id)
////////////        {
////////////            return await _dbContext.Cities.FirstOrDefaultAsync(x => x.Id == id);
////////////        }

////////////        public async Task<City> CreateAsync(City city)
////////////        {
////////////            await _dbContext.Cities.AddAsync(city);
////////////            await _dbContext.SaveChangesAsync();
////////////            return city;
////////////        }

////////////        public async Task<City?> UpdateAsync(Guid id, City city)
////////////        {
////////////            var existingCity = await _dbContext.Cities.FirstOrDefaultAsync(x => x.Id == id);

////////////            if (existingCity == null)
////////////            {
////////////                return null;
////////////            }

////////////            existingCity.Code = city.Code;
////////////            existingCity.Name = city.Name;
////////////            existingCity.CityImageUrl = city.CityImageUrl;

////////////            await _dbContext.SaveChangesAsync();
////////////            return existingCity;
////////////        }

////////////        public async Task<City?> DeleteAsync(Guid id)
////////////        {
////////////            var existingCity = await _dbContext.Cities.FirstOrDefaultAsync(x => x.Id == id);

////////////            if (existingCity == null)
////////////            {
////////////                return null;
////////////            }

////////////            _dbContext.Cities.Remove(existingCity);
////////////            await _dbContext.SaveChangesAsync();
////////////            return existingCity;
////////////        }
////////////    }
////////////}




