namespace FoodieHub.API.Models.DTO
{
    public class RefreshResponseDto
    {
        public string JwtToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
