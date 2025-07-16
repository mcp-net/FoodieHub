// UpdatePriceRangeRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTO
{
    public class UpdatePriceRangeRequestDto
    {
        [Required]
        public string Name { get; set; }
    }
}