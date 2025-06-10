using ECommercePlatform.API.Middleware;
using ECommercePlatform.Application.Features.User.Commands.Create;
using ECommercePlatform.Application.Features.User.Commands.Delete;
using ECommercePlatform.Application.Features.User.Commands.Update;
using ECommercePlatform.Application.Features.User.Queries.GetAllUsers;
using ECommercePlatform.Application.Features.User.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [HasPermission("User", "View")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        [HasPermission("User", "View")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [HasPermission("User", "Create")]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpPut("{id}")]
        [HasPermission("User", "Edit")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id) return BadRequest();
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [HasPermission("User", "Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));
            if (!result) return NotFound();
            return NoContent();
        }
    }
}