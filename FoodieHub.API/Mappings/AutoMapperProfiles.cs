using AutoMapper;
using FoodieHub.API.Models.Domain;
using FoodieHub.API.Models.DTO;

namespace FoodieHub.API.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<City, CityDto>().ReverseMap();
            CreateMap<AddCityRequestDto, City>().ReverseMap();
            CreateMap<UpdateCityRequestDto, City>().ReverseMap();

            CreateMap<AddRestaurantRequestDto, Restaurant>().ReverseMap();
            CreateMap<Restaurant, RestaurantDto>().ReverseMap();
            CreateMap<PriceRange, PriceRangeDto>().ReverseMap();
            CreateMap<UpdateRestaurantRequestDto, Restaurant>().ReverseMap();


            CreateMap<AddPriceRangeRequestDto, PriceRange>();
            CreateMap<UpdatePriceRangeRequestDto, PriceRange>();



        }
    }
}