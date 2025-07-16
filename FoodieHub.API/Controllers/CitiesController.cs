using AutoMapper;
using FoodieHub.API.Authentication;
using FoodieHub.API.CustomActionFilters;
using FoodieHub.API.Models.Domain;
using FoodieHub.API.Models.DTO;
using FoodieHub.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CitiesController> _logger;

        public CitiesController(ICityRepository cityRepository, IMapper mapper,
            ILogger<CitiesController> logger)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // GET ALL CITIES
        // GET: https://localhost:portnumber/api/cities
        [HttpGet]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "Reviewer")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GetAllCities Action Method was invoked");

            // Get Data From Database - Domain models
            var citiesDomain = await _cityRepository.GetAllAsync();

            _logger.LogInformation($"Finished GetAllCities request with data: {JsonSerializer.Serialize(citiesDomain)}");

            // Map Domain Models to DTOs
            var citiesDto = _mapper.Map<List<CityDto>>(citiesDomain);

            // Return DTOs
            return Ok(citiesDto);
        }


        [HttpGet]
        [Authorize(Roles = "Reviewer")]
        public async Task<IActionResult> GetAll(
                        [FromQuery] string? search,
                        [FromQuery] int page = 1,
                        [FromQuery] int pageSize = 20)
        {
            var (items, total) = await _cityRepository.GetPagedAsync(search, page, pageSize);


            //Response.Headers.Add("X-Total-Count", total.ToString());

            // New:
            //Response.Headers.Append("X-Total-Count", total.ToString());

            //Add will throw if the header already exists; Append will concatenate.
            //Alternatively, to overwrite:

            Response.Headers["X-Total-Count"] = total.ToString();





            var dtos = _mapper.Map<List<CityDto>>(items);
            return Ok(dtos);
        }

        [HttpGet("code/{code}")]
        [Authorize(Roles = "Reviewer")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var city = await _cityRepository.GetByCodeAsync(code);
            if (city == null) return NotFound();
            return Ok(_mapper.Map<CityDto>(city));
        }

        [HttpPost("{id:guid}/image")]
        [Authorize(Roles = "RestaurantOwner")]
        public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
        {
            var url = await _cityRepository.SaveImageAsync(id, file);
            return Ok(new { ImageUrl = url });
        }

        [HttpHead]
        public async Task<IActionResult> Head() => Ok(new { Count = await _cityRepository.CountAsync() });


        // GET SINGLE CITY (Get City By ID)
        // GET: https://localhost:portnumber/api/cities/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "Reviewer")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            // Get City Domain Model From Database
            var cityDomain = await _cityRepository.GetByIdAsync(id);

            if (cityDomain == null)
            {
                return NotFound();
            }

            // Map City Domain Model to City DTO
            var cityDto = _mapper.Map<CityDto>(cityDomain);

            // Return DTO back to client
            return Ok(cityDto);
        }

        // POST To Create New City
        // POST: https://localhost:portnumber/api/cities
        [HttpPost]
        [ValidateModel]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "RestaurantOwner")]
        public async Task<IActionResult> Create([FromBody] AddCityRequestDto addCityRequestDto)
        {
            // Map DTO to Domain Model
            var cityDomainModel = _mapper.Map<City>(addCityRequestDto);

            // Use Domain Model to create City
            cityDomainModel = await _cityRepository.CreateAsync(cityDomainModel);

            // Map Domain model back to DTO
            var cityDto = _mapper.Map<CityDto>(cityDomainModel);

            return CreatedAtAction(nameof(GetById), new { id = cityDto.Id }, cityDto);
        }

        // Update city
        // PUT: https://localhost:portnumber/api/cities/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "RestaurantOwner")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCityRequestDto updateCityRequestDto)
        {
            // Map DTO to Domain Model
            var cityDomainModel = _mapper.Map<City>(updateCityRequestDto);

            // Check if city exists
            cityDomainModel = await _cityRepository.UpdateAsync(id, cityDomainModel);

            if (cityDomainModel == null)
            {
                return NotFound();
            }

            // Map Domain Model to DTO
            var cityDto = _mapper.Map<CityDto>(cityDomainModel);

            return Ok(cityDto);
        }

        // Delete City
        // DELETE: https://localhost:portnumber/api/cities/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "RestaurantOwner")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var cityDomainModel = await _cityRepository.DeleteAsync(id);

            if (cityDomainModel == null)
            {
                return NotFound();
            }

            // Map Domain Model to DTO
            var cityDto = _mapper.Map<CityDto>(cityDomainModel);

            return Ok(cityDto);
        }



        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "RestaurantOwner")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] JsonPatchDocument<UpdateCityRequestDto> patchDoc)
        {
            var existing = await _cityRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var dto = _mapper.Map<UpdateCityRequestDto>(existing);


            //patchDoc.ApplyTo(dto, ModelState);

            // Apply patch and capture errors into ModelState
            patchDoc.ApplyTo(dto, error =>
                ModelState.AddModelError(error.Operation.path, error.ErrorMessage));


            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var updatedDomain = _mapper.Map<City>(dto);
            var saved = await _cityRepository.UpdateAsync(id, updatedDomain);
            return Ok(_mapper.Map<CityDto>(saved));
        }



    }
}