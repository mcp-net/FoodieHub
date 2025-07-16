// AddPriceRangeRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTO
{
    public class AddPriceRangeRequestDto
    {
        [Required]
        public string Name { get; set; }
    }
}