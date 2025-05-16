using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces.ICountry;
using ECommercePlatform.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCountries()
        {
            var countries = await _countryService.GetAllCountriesAsync();
            return Ok(countries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountryById(Guid id)
        {
            try
            {
                var country = await _countryService.GetCountryByIdAsync(id);
                return Ok(country);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/states")]
        public async Task<IActionResult> GetCountryWithStates(Guid id)
        {
            try
            {
                var country = await _countryService.GetCountryWithStatesAsync(id);
                return Ok(country);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDto createCountryDto)
        {
            try
            {
                var country = await _countryService.CreateCountryAsync(createCountryDto);
                return CreatedAtAction(nameof(GetCountryById), new { id = country.Id }, country);
            }
            catch (DuplicateResourceException ex)
            {
                // Return 409 Conflict with the error message
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCountry(Guid id, [FromBody] UpdateCountryDto updateCountryDto)
        {
            try
            {
                await _countryService.UpdateCountryAsync(id, updateCountryDto);
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
        public async Task<IActionResult> DeleteCountry(Guid id)
        {
            try
            {
                await _countryService.DeleteCountryAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}