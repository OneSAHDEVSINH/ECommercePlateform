using ECommercePlatform.API.Middleware;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Features.Role.Commands.AssignPermissions;
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
        [HasPermission("roles", "View")]
        public async Task<IActionResult> GetAllRoles([FromQuery] bool activeOnly = true)
        {
            var result = await _mediator.Send(new GetAllRolesQuery(activeOnly));

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("paged")]
        [HasPermission("roles", "View")]
        public async Task<IActionResult> GetPagedRoles([FromQuery] GetPagedRolesQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("{id}")]
        [HasPermission("roles", "View")]
        public async Task<IActionResult> GetRoleById(Guid id)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpGet("{id}/permissions")]
        [HasPermission("roles", "View")]
        public async Task<IActionResult> GetRoleWithPermissions(Guid id)
        {
            var result = await _mediator.Send(new GetRoleWithPermissionsQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpPost]
        [HasPermission("roles", "Add")]
        public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
        {
            if (command == null)
            {
                // Add logging here
                return BadRequest(new { message = "Request body cannot be null" });
            }
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetRoleById), new { id = result.Value.Id }, result.Value);

            return Conflict(new { message = result.Error });
        }

        [HttpPut("{id}")]
        [HasPermission("roles", "Edit")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleCommand command)
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
        [HasPermission("roles", "Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(id));

            if (result.IsSuccess)
                return NoContent();

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }

        [HttpPost("{id}/permissions")]
        [HasPermission("roles", "Edit")]
        public async Task<IActionResult> AssignPermissions(Guid id, [FromBody] List<AssignPermissionDto> permissions)
        {
            var command = new AssignPermissionsToRoleCommand
            {
                RoleId = id,
                Permissions = permissions
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }
    }
}