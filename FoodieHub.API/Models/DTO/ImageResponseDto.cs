// Models/DTO/ImageResponseDto.cs
namespace FoodieHub.API.Models.DTO
{
    public class ImageResponseDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FileExtension { get; set; } = null!;
        public long FileSizeInBytes { get; set; }
        public string FilePath { get; set; } = null!;
        public string? FileDescription { get; set; }
    }
}
