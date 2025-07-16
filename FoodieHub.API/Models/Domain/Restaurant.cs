namespace FoodieHub.API.Models.Domain
{
    public class Restaurant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string? RestaurantImageUrl { get; set; }
        public double Rating { get; set; }

        public Guid PriceRangeId { get; set; }
        public Guid CityId { get; set; }

        // Navigation properties
        public PriceRange PriceRange { get; set; }
        public City City { get; set; }
    }
}