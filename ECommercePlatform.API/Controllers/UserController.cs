// API/Controllers/UserController.cs
using ECommercePlatform.API.Extensions;
using ECommercePlatform.Application.Common.Authorization.Attributes;
using ECommercePlatform.Application.Features.Users.Commands.AssignRolesToUser;
using ECommercePlatform.Application.Features.Users.Commands.ChangePassword;
using ECommercePlatform.Application.Features.Users.Commands.Create;
using ECommercePlatform.Application.Features.Users.Commands.Delete;
using ECommercePlatform.Application.Features.Users.Commands.RemoveAvatar;
using ECommercePlatform.Application.Features.Users.Commands.Update;
using ECommercePlatform.Application.Features.Users.Commands.UpdateUserProfile;
using ECommercePlatform.Application.Features.Users.Commands.UploadAvatar;
using ECommercePlatform.Application.Features.Users.Queries.GetAllUsers;
using ECommercePlatform.Application.Features.Users.Queries.GetCurrentUserProfile;
using ECommercePlatform.Application.Features.Users.Queries.GetPagedUsers;
using ECommercePlatform.Application.Features.Users.Queries.GetUserByEmail;
using ECommercePlatform.Application.Features.Users.Queries.GetUserById;
using ECommercePlatform.Application.Features.Users.Queries.GetUsersByRoleId;
using ECommercePlatform.Application.Features.Users.Queries.GetUserWithRoles;
using ECommercePlatform.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPermissionService _permissionService;

        public UserController(IMediator mediator, IPermissionService permissionService)
        {
            _mediator = mediator;
            _permissionService = permissionService;
        }

        // Admin endpoints
        [HttpGet]
        [HasPermission("Users", "View")]
        public async Task<IActionResult> GetAllUsers([FromQuery] bool activeOnly = true)
        {
            var result = await _mediator.Send(new GetAllUsersQuery(activeOnly));

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("paged")]
        [HasPermission("Users", "View")]
        public async Task<IActionResult> GetPagedUsers([FromQuery] GetPagedUsersQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("{id}")]
        [HasPermission("Users", "View")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpGet("{id}/roles")]
        [HasPermission("Users", "View")]
        public async Task<IActionResult> GetUserWithRoles(Guid id)
        {
            var result = await _mediator.Send(new GetUserWithRolesQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpGet("by-email")]
        [HasPermission("Users", "View")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            var result = await _mediator.Send(new GetUserByEmailQuery(email));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpGet("by-role/{roleId}")]
        [HasPermission("Users", "View")]
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
        [HasPermission("Users", "AddEdit")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetUserById), new { id = result.Value.Id }, result.Value);

            return Conflict(new { message = result.Error });
        }

        [HttpPut("{id}")]
        [HasPermission("Users", "AddEdit")]
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
        [HasPermission("Users", "Delete")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));

            if (result.IsSuccess)
                return NoContent();

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }

        [HttpPost("{id}/roles")]
        [HasPermission("Users", "Edit")]
        public async Task<IActionResult> AssignRoles(Guid id, [FromBody] List<Guid> roleIds)
        {
            var command = new AssignRolesToUserCommand
            {
                UserId = id,
                RoleIds = roleIds
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return NoContent();

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            var userId = this.GetCurrentUserId();

            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _mediator.Send(new GetCurrentUserProfileQuery(userId.Value.ToString()));

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateCurrentUserProfile([FromBody] UpdateUserProfileCommand command)
        {
            var userId = this.GetCurrentUserId();

            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            command.Id = userId.Value;

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpPost("profile/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated" });

            if (!Guid.TryParse(userId, out var userGuid))
                return BadRequest(new { message = "Invalid user ID format" });

            command.UserId = userGuid;

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(new { message = "Password changed successfully" });

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("profile/permissions")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserPermissions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated" });

            if (!Guid.TryParse(userId, out var userGuid))
                return BadRequest(new { message = "Invalid user ID format" });

            var permissions = await _permissionService.GetUserPermissionsAsync(userGuid);

            return Ok(new { permissions });
        }

        //[HttpPost("profile/upload-avatar")]
        //[Authorize]
        //public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized(new { message = "User not authenticated" });

        //    if (!Guid.TryParse(userId, out var userGuid))
        //        return BadRequest(new { message = "Invalid user ID format" });

        //    if (file == null || file.Length == 0)
        //        return BadRequest(new { message = "No file uploaded" });

        //    // Validate file type
        //    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
        //    if (!allowedTypes.Contains(file.ContentType))
        //        return BadRequest(new { message = "Invalid file type. Only JPEG, PNG, and GIF are allowed." });

        //    // Validate file size (5MB max)
        //    if (file.Length > 5 * 1024 * 1024)
        //        return BadRequest(new { message = "File size must not exceed 5MB" });

        //    var command = new UploadAvatarCommand
        //    {
        //        UserId = userGuid,
        //        File = file
        //    };

        //    var result = await _mediator.Send(command);

        //    if (result.IsSuccess)
        //        return Ok(new { message = "Avatar uploaded successfully", avatarUrl = result.Value });

        //    return BadRequest(new { message = result.Error });
        //}

        //[HttpDelete("profile/avatar")]
        //[Authorize]
        //public async Task<IActionResult> RemoveAvatar()
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized(new { message = "User not authenticated" });

        //    if (!Guid.TryParse(userId, out var userGuid))
        //        return BadRequest(new { message = "Invalid user ID format" });

        //    var command = new RemoveAvatarCommand { UserId = userGuid };

        //    var result = await _mediator.Send(command);

        //    if (result.IsSuccess)
        //        return Ok(new { message = "Avatar removed successfully" });

        //    return BadRequest(new { message = result.Error });
        //}
    }
}