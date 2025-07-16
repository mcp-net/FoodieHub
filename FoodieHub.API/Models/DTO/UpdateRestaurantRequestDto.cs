using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTO
{
    public class UpdateRestaurantRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        public string? RestaurantImageUrl { get; set; }

        [Required]
        [Range(0, 5)]
        public double Rating { get; set; }

        [Required]
        public Guid PriceRangeId { get; set; }

        [Required]
        public Guid CityId { get; set; }
    }
}