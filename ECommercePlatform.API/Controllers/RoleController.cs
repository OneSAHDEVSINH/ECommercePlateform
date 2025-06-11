using ECommercePlatform.API.Middleware;
using ECommercePlatform.Application.Features.Role.Commands.Create;
using ECommercePlatform.Application.Features.Role.Commands.Delete;
using ECommercePlatform.Application.Features.Role.Commands.Update;
using ECommercePlatform.Application.Features.Role.Queries.GetAllRoles;
using ECommercePlatform.Application.Features.Role.Queries.GetPagedRoles;
using ECommercePlatform.Application.Features.Role.Queries.GetRoleById;
using ECommercePlatform.Application.Features.Role.Queries.GetRoleWithPermissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoleController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [HasPermission("Role", "View")]
        public async Task<IActionResult> GetAllRoles([FromQuery] bool activeOnly = true)
        {
            var result = await _mediator.Send(new GetAllRolesQuery(activeOnly));

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("paged")]
        [HasPermission("Role", "View")]
        public async Task<IActionResult> GetPagedRoles([FromQuery] GetPagedRolesQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("{id}")]
        [HasPermission("Role", "View")]
        public async Task<IActionResult> GetRoleById(Guid id)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpGet("{id}/permissions")]
        [HasPermission("Role", "View")]
        public async Task<IActionResult> GetRoleWithPermissions(Guid id)
        {
            var result = await _mediator.Send(new GetRoleWithPermissionsQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpPost]
        [HasPermission("Role", "Create")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetRoleById), new { id = result.Value.Id }, result.Value);

            return Conflict(new { message = result.Error });
        }

        [HttpPut("{id}")]
        [HasPermission("Role", "Edit")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleCommand command)
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
        [HasPermission("Role", "Delete")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(id));

            if (result.IsSuccess)
                return NoContent();

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }
    }
}