using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodieHub.API.CustomActionFilters;
using FoodieHub.API.Models.Domain;
using FoodieHub.API.Models.DTO;
using FoodieHub.API.Repositories;
using FoodieHub.API.Authentication;

namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRestaurantRepository _restaurantRepository;

        public RestaurantsController(IMapper mapper, IRestaurantRepository restaurantRepository)
        {
            _mapper = mapper;
            _restaurantRepository = restaurantRepository;
        }

        // CREATE Restaurant
        // POST: /api/restaurants
        [HttpPost]
        [ValidateModel]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "RestaurantOwner")]
        public async Task<IActionResult> Create([FromBody] AddRestaurantRequestDto addRestaurantRequestDto)
        {
            // Map DTO to Domain Model
            var restaurantDomainModel = _mapper.Map<Restaurant>(addRestaurantRequestDto);

            await _restaurantRepository.CreateAsync(restaurantDomainModel);

            // Map Domain model to DTO
            return Ok(_mapper.Map<RestaurantDto>(restaurantDomainModel));
        }

        // GET Restaurants
        // GET: /api/restaurants?filterOn=Name&filterQuery=Pizza&sortBy=Rating&isAscending=false&pageNumber=1&pageSize=10
        [HttpGet]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "Reviewer")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var restaurantsDomainModel = await _restaurantRepository.GetAllAsync(filterOn, filterQuery, sortBy,
                isAscending ?? true, pageNumber, pageSize);

            // Map Domain Model to DTO
            return Ok(_mapper.Map<List<RestaurantDto>>(restaurantsDomainModel));
        }

        // Get Restaurant By Id
        // GET: /api/restaurants/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "Reviewer")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var restaurantDomainModel = await _restaurantRepository.GetByIdAsync(id);

            if (restaurantDomainModel == null)
            {
                return NotFound();
            }

            // Map Domain Model to DTO
            return Ok(_mapper.Map<RestaurantDto>(restaurantDomainModel));
        }

        // Update Restaurant By Id
        // PUT: /api/restaurants/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "RestaurantOwner")]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateRestaurantRequestDto updateRestaurantRequestDto)
        {
            // Map DTO to Domain Model
            var restaurantDomainModel = _mapper.Map<Restaurant>(updateRestaurantRequestDto);

            restaurantDomainModel = await _restaurantRepository.UpdateAsync(id, restaurantDomainModel);

            if (restaurantDomainModel == null)
            {
                return NotFound();
            }

            // Map Domain model to DTO
            return Ok(_mapper.Map<RestaurantDto>(restaurantDomainModel));
        }

        // Delete a Restaurant By Id
        // DELETE: /api/restaurants/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "RestaurantOwner")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedRestaurantDomainModel = await _restaurantRepository.DeleteAsync(id);

            if (deletedRestaurantDomainModel == null)
            {
                return NotFound();
            }

            // Map Domain Model to DTO
            return Ok(_mapper.Map<RestaurantDto>(deletedRestaurantDomainModel));
        }
    }
}