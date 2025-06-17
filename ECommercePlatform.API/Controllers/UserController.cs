using ECommercePlatform.Application.Common.Authorization.Attributes;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Features.User.Commands.Create;
using ECommercePlatform.Application.Features.User.Commands.Delete;
using ECommercePlatform.Application.Features.User.Commands.Update;
using ECommercePlatform.Application.Features.User.Queries.GetAllUsers;
using ECommercePlatform.Application.Features.User.Queries.GetPagedUsers;
using ECommercePlatform.Application.Features.User.Queries.GetUserById;
using ECommercePlatform.Application.Features.User.Queries.GetUsersByRoleId;
using ECommercePlatform.Application.Features.User.Queries.GetUserWithRoles;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController(IMediator mediator, IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet]
        [HasPermission("users", "View")]
        public async Task<IActionResult> GetAllUsers([FromQuery] bool activeOnly = true)
        {
            var result = await _mediator.Send(new GetAllUsersQuery(activeOnly));

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("paged")]
        [HasPermission("users", "View")]
        public async Task<IActionResult> GetPagedUsers([FromQuery] GetPagedUsersQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { message = result.Error });
        }

        [HttpGet("{id}")]
        [HasPermission("users", "View")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        [HttpGet("{id}/roles")]
        [HasPermission("users", "View")]
        public async Task<IActionResult> GetUserWithRoles(Guid id)
        {
            var result = await _mediator.Send(new GetUserWithRolesQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { message = result.Error });
        }

        //[HttpGet("by-email")]
        //[HasPermission("users", "View")]
        //public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        //{
        //    var result = await _mediator.Send(new GetUserByEmailQuery(email));

        //    if (result.IsSuccess)
        //        return Ok(result.Value);

        //    return NotFound(new { message = result.Error });
        //}

        [HttpGet("by-role/{roleId}")]
        [HasPermission("users", "View")]
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
        [HasPermission("users", "Add")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetUserById), new { id = result.Value.Id }, result.Value);

            return Conflict(new { message = result.Error });
        }

        [HttpPut("{id}")]
        [HasPermission("users", "Edit")]
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
        [HasPermission("users", "Delete")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));

            if (result.IsSuccess)
                return NoContent();

            return result.Error.Contains("not found")
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }

        [HttpGet("{id}")]
        [HasPermission("users", "View")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Map to DTO
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            };

            return Ok(userDto);
        }

        [HttpGet("{id}/roles")]
        [HasPermission("users", "View")]
        public async Task<IActionResult> GetUserWithRoles(string id)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var roles = await _unitOfWork.UserManager.GetRolesAsync(user);
            var roleDtos = new List<RoleDto>();

            foreach (var roleName in roles)
            {
                var role = await _unitOfWork.RoleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    roleDtos.Add(new RoleDto
                    {
                        Id = role.Id,
                        Name = role.Name,
                        Description = role.Description,
                        IsActive = role.IsActive
                    });
                }
            }

            // Map to DTO
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Roles = roleDtos
            };

            return Ok(userDto);
        }

        [HttpGet("by-email")]
        [HasPermission("users", "View")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Map to DTO
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            };

            return Ok(userDto);
        }

        [HttpGet("by-role/{roleId}")]
        [HasPermission("users", "View")]
        public async Task<IActionResult> GetUsersByRoleId(string roleId, [FromQuery] bool activeOnly = true)
        {
            var role = await _unitOfWork.RoleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound(new { message = "Role not found" });

            var usersInRole = await _unitOfWork.UserManager.GetUsersInRoleAsync(role.Name);

            // Filter by active status if required
            if (activeOnly)
                usersInRole = usersInRole.Where(u => u.IsActive && !u.IsDeleted).ToList();

            // Map to DTOs
            var userDtos = usersInRole.Select(user => new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            }).ToList();

            return Ok(userDtos);
        }

        [HttpPost]
        [HasPermission("users", "Add")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                IsActive = model.IsActive
            };

            var result = await _unitOfWork.UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

            // Assign roles if provided
            if (model.RoleIds != null && model.RoleIds.Any())
            {
                foreach (var roleId in model.RoleIds)
                {
                    var role = await _unitOfWork.RoleManager.FindByIdAsync(roleId);
                    if (role != null)
                    {
                        await _unitOfWork.UserManager.AddToRoleAsync(user, role.Name);
                    }
                }
            }

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            });
        }

        [HttpPut("{id}")]
        [HasPermission("users", "Edit")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.IsActive = model.IsActive;

            // Only update email if it changed
            if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailOwner = await _unitOfWork.UserManager.FindByEmailAsync(model.Email);
                if (emailOwner != null && !string.Equals(emailOwner.Id, id))
                    return BadRequest(new { message = "Email already in use" });

                user.UserName = model.Email;
                user.Email = model.Email;
            }

            var result = await _unitOfWork.UserManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

            // Update password if provided
            if (!string.IsNullOrEmpty(model.Password))
            {
                await _unitOfWork.UserManager.RemovePasswordAsync(user);
                result = await _unitOfWork.UserManager.AddPasswordAsync(user, model.Password);

                if (!result.Succeeded)
                    return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [HasPermission("users", "Delete")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Soft delete
            user.IsDeleted = true;
            user.IsActive = false;

            var result = await _unitOfWork.UserManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

            return NoContent();
        }

        [HttpPost("{id}/roles")]
        [HasPermission("users", "Edit")]
        public async Task<IActionResult> AssignRoles(string id, [FromBody] UserRoleAssignmentDto model)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Remove existing roles
            var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);
            await _unitOfWork.UserManager.RemoveFromRolesAsync(user, userRoles);

            // Add new roles
            foreach (var roleId in model.RoleIds)
            {
                var role = await _unitOfWork.RoleManager.FindByIdAsync(roleId);
                if (role != null)
                {
                    await _unitOfWork.UserManager.AddToRoleAsync(user, role.Name);
                }
            }

            return NoContent();
        }

        [HttpPost("{id}/reset-password")]
        [HasPermission("users", "Edit")]
        public async Task<IActionResult> ResetPassword(string id, [FromBody] PasswordResetDto model)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            await _unitOfWork.UserManager.RemovePasswordAsync(user);
            var result = await _unitOfWork.UserManager.AddPasswordAsync(user, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

            return NoContent();
        }
    }

    public class CreateUserDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public List<string>? RoleIds { get; set; }
    }

    public class UpdateUserDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UserRoleAssignmentDto
    {
        public required List<string> RoleIds { get; set; } = new();
    }

    public class PasswordResetDto
    {
        public required string NewPassword { get; set; }
    }
}