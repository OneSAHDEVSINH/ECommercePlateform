using ECommercePlatform.API.Middleware;
using ECommercePlatform.Application.Features.User.Commands.Create;
using ECommercePlatform.Application.Features.User.Commands.Delete;
using ECommercePlatform.Application.Features.User.Commands.Update;
using ECommercePlatform.Application.Features.User.Queries.GetAllUsers;
using ECommercePlatform.Application.Features.User.Queries.GetPagedUsers;
using ECommercePlatform.Application.Features.User.Queries.GetUserByEmail;
using ECommercePlatform.Application.Features.User.Queries.GetUserById;
using ECommercePlatform.Application.Features.User.Queries.GetUsersByRoleId;
using ECommercePlatform.Application.Features.User.Queries.GetUserWithRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class UserController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        //[HasPermission("User", "View")]
        public async Task<IActionResult> GetAllUsers([FromQuery] bool activeOnly = true)
        {
            var result = await _mediator.Send(new GetAllUsersQuery(activeOnly));

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("paged")]
        //[HasPermission("User", "View")]
        public async Task<IActionResult> GetPagedUsers([FromQuery] GetPagedUsersQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("{id}")]
        //[HasPermission("User", "View")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpGet("{id}/roles")]
        //[HasPermission("User", "View")]
        public async Task<IActionResult> GetUserWithRoles(Guid id)
        {
            var result = await _mediator.Send(new GetUserWithRolesQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpGet("by-email")]
        //[HasPermission("User", "View")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            var result = await _mediator.Send(new GetUserByEmailQuery(email));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpGet("by-role/{roleId}")]
        //[HasPermission("User", "View")]
        public async Task<IActionResult> GetUsersByRoleId(Guid roleId, [FromQuery] bool activeOnly = true)
        {
            var result = await _mediator.Send(new GetUsersByRoleIdQuery(roleId, activeOnly));

            if (result.IsSuccess)
                return Ok(result.Value);

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }

        [HttpPost]
        //[HasPermission("User", "Add")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetUserById), new { id = result.Value.Id }, result.Value);

            return Conflict(new { message = result.Error });
        }

        [HttpPut("{id}")]
        //[HasPermission("User", "Edit")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
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
        //[HasPermission("User", "Delete")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));

            if (result.IsSuccess)
                return NoContent();

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }
    }
}