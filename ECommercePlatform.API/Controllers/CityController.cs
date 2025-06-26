using ECommercePlatform.Application.Common.Authorization.Attributes;
using ECommercePlatform.Application.Features.Cities.Commands.Create;
using ECommercePlatform.Application.Features.Cities.Commands.Delete;
using ECommercePlatform.Application.Features.Cities.Commands.Update;
using ECommercePlatform.Application.Features.Cities.Queries.GetAllCities;
using ECommercePlatform.Application.Features.Cities.Queries.GetCitiesByState;
using ECommercePlatform.Application.Features.Cities.Queries.GetCityById;
using ECommercePlatform.Application.Features.Cities.Queries.GetPagedCities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CityController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [HasPermission("Cities", "View")]
        public async Task<IActionResult> GetAllCities()
        {
            var result = await _mediator.Send(new GetAllCitiesQuery());

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("paged")]
        [HasPermission("Cities", "View")]
        public async Task<IActionResult> GetPagedCities([FromQuery] GetPagedCitiesQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("{id}")]
        [HasPermission("Cities", "View")]
        public async Task<IActionResult> GetCityById(Guid id)
        {
            var result = await _mediator.Send(new GetCityByIdQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpPost]
        [HasPermission("Cities", "AddEdit")]
        public async Task<IActionResult> CreateCity([FromBody] CreateCityCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetCityById), new { id = result.Value.Id }, result.Value);

            return Conflict(new { message = result.Error });
        }

        [HttpPut("{id}")]
        [HasPermission("Cities", "AddEdit")]
        public async Task<IActionResult> UpdateCity(Guid id, [FromBody] UpdateCityCommand command)
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
        [HasPermission("Cities", "Delete")]
        public async Task<IActionResult> DeleteCity(Guid id)
        {
            var result = await _mediator.Send(new DeleteCityCommand(id));

            if (result.IsSuccess)
                return NoContent();

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }

        [HttpGet("ByState/{stateId}")]
        [HasPermission("Cities", "View")]
        public async Task<IActionResult> GetCitiesByState(Guid stateId)
        {
            var result = await _mediator.Send(new GetCitiesByStateQuery(stateId));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }
    }
}