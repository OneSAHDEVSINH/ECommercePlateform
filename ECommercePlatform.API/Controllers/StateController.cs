using ECommercePlatform.Application.Common.Authorization.Attributes;
using ECommercePlatform.Application.Features.States.Commands.Create;
using ECommercePlatform.Application.Features.States.Commands.Delete;
using ECommercePlatform.Application.Features.States.Commands.Update;
using ECommercePlatform.Application.Features.States.Queries.GetAllStates;
using ECommercePlatform.Application.Features.States.Queries.GetPagedStates;
using ECommercePlatform.Application.Features.States.Queries.GetStatesByCountry;
using ECommercePlatform.Application.Features.States.Queries.GetStatesById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StateController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [HasPermission("States", "View")]
        public async Task<IActionResult> GetAllStates()
        {
            var result = await _mediator.Send(new GetAllStatesQuery());

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("paged")]
        [HasPermission("States", "View")]
        public async Task<IActionResult> GetPagedStates([FromQuery] GetPagedStatesQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("{id}")]
        [HasPermission("States", "View")]
        public async Task<IActionResult> GetStateById(Guid id)
        {
            var result = await _mediator.Send(new GetStateByIdQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpPost]
        [HasPermission("States", "AddEdit")]
        public async Task<IActionResult> CreateState([FromBody] CreateStateCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetStateById), new { id = result.Value.Id }, result.Value);

            return Conflict(new { message = result.Error });
        }

        [HttpPut("{id}")]
        [HasPermission("States", "AddEdit")]
        public async Task<IActionResult> UpdateState(Guid id, [FromBody] UpdateStateCommand command)
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
        [HasPermission("States", "Delete")]
        public async Task<IActionResult> DeleteState(Guid id)
        {
            var result = await _mediator.Send(new DeleteStateCommand(id));

            if (result.IsSuccess)
                return NoContent();

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }

        [HttpGet("ByCountry/{countryId}")]
        [HasPermission("States", "View")]
        public async Task<IActionResult> GetStatesByCountry(Guid countryId)
        {
            var result = await _mediator.Send(new GetStatesByCountryQuery(countryId));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }
    }
}
