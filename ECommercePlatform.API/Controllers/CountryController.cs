using ECommercePlatform.API.Middleware;
using ECommercePlatform.Application.Features.Countries.Commands.Create;
using ECommercePlatform.Application.Features.Countries.Commands.Delete;
using ECommercePlatform.Application.Features.Countries.Commands.Update;
using ECommercePlatform.Application.Features.Countries.Queries.GetAllCountries;
using ECommercePlatform.Application.Features.Countries.Queries.GetCountryById;
using ECommercePlatform.Application.Features.Countries.Queries.GetPagedCountries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [HasPermission("Country", "View")]
        public async Task<IActionResult> GetAllCountries()
        {
            var result = await _mediator.Send(new GetAllCountriesQuery());

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedCountries([FromQuery] GetPagedCountriesQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("{id}")]
        //[HasPermission("Country", "View")]
        public async Task<IActionResult> GetCountryById(Guid id)
        {
            var result = await _mediator.Send(new GetCountryByIdQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [HasPermission("Country", "Create")]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetCountryById), new { id = result.Value.Id }, result.Value);

            return Conflict(new { message = result.Error });
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        [HasPermission("Country", "Edit")]
        public async Task<IActionResult> UpdateCountry(Guid id, [FromBody] UpdateCountryCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Id in the URL does not match the Id in the request body" });

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return NoContent();

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : Conflict(new { message = result.Error });
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        [HasPermission("Country", "Delete")]
        public async Task<IActionResult> DeleteCountry(Guid id)
        {
            var result = await _mediator.Send(new DeleteCountryCommand(id));

            if (result.IsSuccess)
                return NoContent();

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllCountries()
        //{
        //    var countries = await _countryService.GetAllCountriesAsync();
        //    return Ok(countries);
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetCountryById(Guid id)
        //{
        //    try
        //    {
        //        var country = await _countryService.GetCountryByIdAsync(id);
        //        return Ok(country);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //}

        //[HttpGet("{id}/states")]
        //public async Task<IActionResult> GetCountryWithStates(Guid id)
        //{
        //    try
        //    {
        //        var country = await _countryService.GetCountryWithStatesAsync(id);
        //        return Ok(country);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDto createCountryDto)
        //{
        //    try
        //    {
        //        var country = await _countryService.CreateCountryAsync(createCountryDto);
        //        return CreatedAtAction(nameof(GetCountryById), new { id = country.Id }, country);
        //    }
        //    catch (DuplicateResourceException ex)
        //    {
        //        // Return 409 Conflict with the error message
        //        return Conflict(new { message = ex.Message });
        //    }
        //}

        //[HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> UpdateCountry(Guid id, [FromBody] UpdateCountryDto updateCountryDto)
        //{
        //    try
        //    {
        //        await _countryService.UpdateCountryAsync(id, updateCountryDto);
        //        return NoContent();
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //    catch (DuplicateResourceException ex)
        //    {
        //        // Return 409 Conflict with the error message
        //        return Conflict(new { message = ex.Message });
        //    }
        //}

        //[HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> DeleteCountry(Guid id)
        //{
        //    try
        //    {
        //        await _countryService.DeleteCountryAsync(id);
        //        return NoContent();
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //}
    }
}