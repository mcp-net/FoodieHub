// PriceRangesController.cs
using AutoMapper;
using FoodieHub.API.Authentication;
using FoodieHub.API.Models.Domain;
using FoodieHub.API.Models.DTO;
using FoodieHub.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceRangesController : ControllerBase
    {
        private readonly IPriceRangeRepository _repo;
        private readonly IMapper _mapper;

        public PriceRangesController(
            IPriceRangeRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // GET: /api/PriceRanges
        [HttpGet]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "Reviewer")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            var dtos = _mapper.Map<List<PriceRangeDto>>(list);
            return Ok(dtos);
        }

        // GET: /api/PriceRanges/{id}
        [HttpGet("{id:guid}")]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "Reviewer")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var pr = await _repo.GetByIdAsync(id);
            if (pr == null) return NotFound();
            return Ok(_mapper.Map<PriceRangeDto>(pr));
        }

        // POST: /api/PriceRanges
        [HttpPost]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "RestaurantOwner")]
        public async Task<IActionResult> Create([FromBody] AddPriceRangeRequestDto dto)
        {
            var domain = _mapper.Map<PriceRange>(dto);
            var created = await _repo.CreateAsync(domain);
            var result = _mapper.Map<PriceRangeDto>(created);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: /api/PriceRanges/{id}
        [HttpPut("{id:guid}")]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "RestaurantOwner")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdatePriceRangeRequestDto dto)
        {
            var domain = _mapper.Map<PriceRange>(dto);
            var updated = await _repo.UpdateAsync(id, domain);
            if (updated == null) return NotFound();
            return Ok(_mapper.Map<PriceRangeDto>(updated));
        }

        // DELETE: /api/PriceRanges/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "RestaurantOwner")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
