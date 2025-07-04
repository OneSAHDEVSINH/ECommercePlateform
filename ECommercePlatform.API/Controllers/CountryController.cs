using ECommercePlatform.Application.Common.Authorization.Attributes;
using ECommercePlatform.Application.Features.Countries.Commands.Create;
using ECommercePlatform.Application.Features.Countries.Commands.Delete;
using ECommercePlatform.Application.Features.Countries.Commands.Update;
using ECommercePlatform.Application.Features.Countries.Queries.GetAllCountries;
using ECommercePlatform.Application.Features.Countries.Queries.GetCountryById;
using ECommercePlatform.Application.Features.Countries.Queries.GetPagedCountries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CountryController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [HasPermission("Countries", "View")]
        public async Task<IActionResult> GetAllCountries()
        {
            var result = await _mediator.Send(new GetAllCountriesQuery());

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("paged")]
        [HasPermission("Countries", "View")]
        public async Task<IActionResult> GetPagedCountries([FromQuery] GetPagedCountriesQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("{id}")]
        [HasPermission("Countries", "View")]
        public async Task<IActionResult> GetCountryById(Guid id)
        {
            var result = await _mediator.Send(new GetCountryByIdQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpPost]
        [HasPermission("Countries", "AddEdit")]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetCountryById), new { id = result.Value.Id }, result.Value);

            return Conflict(new { message = result.Error });
        }

        [HttpPut("{id}")]
        [HasPermission("Countries", "AddEdit")]
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
        [HasPermission("Countries", "Delete")]
        public async Task<IActionResult> DeleteCountry(Guid id)
        {
            var result = await _mediator.Send(new DeleteCountryCommand(id));

            if (result.IsSuccess)
                return NoContent();

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }
    }
}