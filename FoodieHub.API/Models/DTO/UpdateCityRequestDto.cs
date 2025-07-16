using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTO
{
    public class UpdateCityRequestDto
    {
        [Required]
        [MinLength(2, ErrorMessage = "Code has to be a minimum of 2 characters")]
        [MaxLength(3, ErrorMessage = "Code has to be a maximum of 3 characters")]
        public string Code { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Name has to be a maximum of 100 characters")]
        public string Name { get; set; }

        public string? CityImageUrl { get; set; }
    }
}