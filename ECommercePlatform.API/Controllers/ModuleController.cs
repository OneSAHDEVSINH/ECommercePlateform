using ECommercePlatform.Application.Common.Authorization.Attributes;
using ECommercePlatform.Application.Features.Modules.Commands.Create;
using ECommercePlatform.Application.Features.Modules.Commands.Delete;
using ECommercePlatform.Application.Features.Modules.Commands.Update;
using ECommercePlatform.Application.Features.Modules.Queries.GetAllModules;
using ECommercePlatform.Application.Features.Modules.Queries.GetModuleById;
using ECommercePlatform.Application.Features.Modules.Queries.GetModuleByRoute;
using ECommercePlatform.Application.Features.Modules.Queries.GetPagedModules;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ModuleController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [HasPermission("Modules", "View")]
        public async Task<IActionResult> GetAllModules([FromQuery] bool activeOnly = true)
        {
            var result = await _mediator.Send(new GetAllModulesQuery(activeOnly));

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("paged")]
        [HasPermission("Modules", "View")]
        public async Task<IActionResult> GetPagedModules([FromQuery] GetPagedModulesQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("{id}")]
        [HasPermission("Modules", "View")]
        public async Task<IActionResult> GetModuleById(Guid id)
        {
            var result = await _mediator.Send(new GetModuleByIdQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpGet("by-route/{route}")]
        [HasPermission("Modules", "View")]
        public async Task<IActionResult> GetModuleByRoute(string route)
        {
            var result = await _mediator.Send(new GetModuleByRouteQuery(route));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        // REMOVED: GetModuleWithPermissions and GetAllModulesWithPermissions endpoints

        [HttpPost]
        [HasPermission("Modules", "Add")]
        public async Task<IActionResult> CreateModule([FromBody] CreateModuleCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetModuleById), new { id = result.Value.Id }, result.Value);

            return Conflict(new { message = result.Error });
        }

        [HttpPut("{id}")]
        [HasPermission("Modules", "Edit")]
        public async Task<IActionResult> UpdateModule(Guid id, [FromBody] UpdateModuleCommand command)
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
        [HasPermission("Modules", "Delete")]
        public async Task<IActionResult> DeleteModule(Guid id)
        {
            var result = await _mediator.Send(new DeleteModuleCommand(id));

            if (result.IsSuccess)
                return NoContent();

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }
    }
}