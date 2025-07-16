namespace FoodieHub.API.Models.Domain
{
    public class City
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? CityImageUrl { get; set; }
    }
}