using ECommercePlatform.API.Middleware;
using ECommercePlatform.Application.Features.Role.Commands.Create;
using ECommercePlatform.Application.Features.Role.Commands.Delete;
using ECommercePlatform.Application.Features.Role.Commands.Update;
using ECommercePlatform.Application.Features.Role.Queries.GetAllRoles;
using ECommercePlatform.Application.Features.Role.Queries.GetRoleById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [HasPermission("Role", "View")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllRolesQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        [HasPermission("Role", "View")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [HasPermission("Role", "Create")]
        public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpPut("{id}")]
        [HasPermission("Role", "Edit")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleCommand command)
        {
            if (id != command.Id) return BadRequest();
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [HasPermission("Role", "Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(id));
            if (!result) return NotFound();
            return NoContent();
        }
    }
}