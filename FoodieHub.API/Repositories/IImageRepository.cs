using FoodieHub.API.Models.Domain;

namespace FoodieHub.API.Repositories
{
    public interface IImageRepository
    {
        //Task<Image> UploadAsync(Image image);


        Task<Image> UploadAsync(Image image);

        Task<Image?> GetByFileNameAsync(string fileName);
        string GetPhysicalPath(string fileName);

        Task<bool> DeleteAsync(string fileName);

        Task<(List<Image> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize);

    }
}