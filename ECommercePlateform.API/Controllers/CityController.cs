using ECommercePlateform.Application.DTOs;
using ECommercePlateform.Application.Interfaces.ICity;
using ECommercePlateform.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlateform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;
        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCities()
        {
            var cities = await _cityService.GetAllCitiesAsync();
            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityById(Guid id)
        {
            try
            {
                var city = await _cityService.GetCityByIdAsync(id);
                return Ok(city);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCity([FromBody] CreateCityDto createCityDto)
        {
            try
            {
                var city = await _cityService.CreateCityAsync(createCityDto);
                return CreatedAtAction(nameof(GetCityById), new { id = city.Id }, city);
            }
            catch (DuplicateResourceException ex)
            {
                // Return 409 Conflict with the error message
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCity(Guid id, [FromBody] UpdateCityDto updateCityDto)
        {
            try
            {
                await _cityService.UpdateCityAsync(id, updateCityDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DuplicateResourceException ex)
            {
                // Return 409 Conflict with the error message
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCity(Guid id)
        {
            try
            {
                await _cityService.DeleteCityAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("ByState/{stateId}")]
        public async Task<IActionResult> GetCitiesByState(Guid stateId)
        {
            try
            {
                var cities = await _cityService.GetCitiesByStateIdAsync(stateId);
                return Ok(cities);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
