namespace FoodieHub.API.Models.DTO
{
    public class RestaurantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string? RestaurantImageUrl { get; set; }
        public double Rating { get; set; }

        public PriceRangeDto PriceRange { get; set; }
        public CityDto City { get; set; }
    }
}