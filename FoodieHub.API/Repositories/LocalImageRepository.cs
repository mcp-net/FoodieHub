// Repositories/LocalImageRepository.cs
using Microsoft.EntityFrameworkCore;
using FoodieHub.API.Data;
using FoodieHub.API.Models.Domain;

namespace FoodieHub.API.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _http;
        private readonly FoodieHubDbContext _db;

        public LocalImageRepository(
            IWebHostEnvironment env,
            IHttpContextAccessor http,
            FoodieHubDbContext db)
        {
            _env = env;
            _http = http;
            _db = db;
        }

        public async Task<Image> UploadAsync(Image image)
        {
            var uploadFolder = Path.Combine(_env.ContentRootPath, "Images");
            Directory.CreateDirectory(uploadFolder);

            var safeName = $"{image.FileName}_{Guid.NewGuid()}{image.FileExtension}";
            var localPath = Path.Combine(uploadFolder, safeName);

            await using var fs = new FileStream(localPath, FileMode.Create);
            await image.File.CopyToAsync(fs);

            var url = $"{_http.HttpContext.Request.Scheme}://"
                    + $"{_http.HttpContext.Request.Host}"
                    + $"/Images/{safeName}";

            image.FilePath = url;
            image.FileName = safeName;    // store the unique name
            await _db.Images.AddAsync(image);
            await _db.SaveChangesAsync();
            return image;
        }

        public async Task<Image?> GetByFileNameAsync(string fileName) =>
            await _db.Images.FirstOrDefaultAsync(img => img.FileName == fileName);

        public string GetPhysicalPath(string fileName)
        {
            var folder = Path.Combine(_env.ContentRootPath, "Images");
            return Path.Combine(folder, fileName);
        }

        public async Task<bool> DeleteAsync(string fileName)
        {
            var img = await GetByFileNameAsync(fileName);
            if (img == null) return false;

            var path = GetPhysicalPath(fileName);
            if (File.Exists(path)) File.Delete(path);

            _db.Images.Remove(img);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(List<Image> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _db.Images.AsQueryable();
            var total = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, total);
        }
    }
}
















//using FoodieHub.API.Data;
//using FoodieHub.API.Models.Domain;

//namespace FoodieHub.API.Repositories
//{
//    public class LocalImageRepository : IImageRepository
//    {
//        private readonly IWebHostEnvironment _webHostEnvironment;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private readonly FoodieHubDbContext _dbContext;

//        public LocalImageRepository(IWebHostEnvironment webHostEnvironment,
//            IHttpContextAccessor httpContextAccessor,
//            FoodieHubDbContext dbContext)
//        {
//            _webHostEnvironment = webHostEnvironment;
//            _httpContextAccessor = httpContextAccessor;
//            _dbContext = dbContext;
//        }

//        public async Task<Image> UploadAsync(Image image)
//        {
//            var localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images",
//                $"{image.FileName}{image.FileExtension}");

//            // Upload Image to Local Path
//            using var stream = new FileStream(localFilePath, FileMode.Create);
//            await image.File.CopyToAsync(stream);

//            // Generate URL to access image
//            var urlFilePath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

//            image.FilePath = urlFilePath;

//            // Add Image to the Images table
//            await _dbContext.Images.AddAsync(image);
//            await _dbContext.SaveChangesAsync();

//            return image;
//        }
//    }
//}



