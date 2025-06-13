using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using System.Security.Claims;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class AuthorizationController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet("check")]
        public async Task<IActionResult> CheckPermission([FromQuery] string moduleRoute, [FromQuery] string permissionType)
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            // Check for super admin
            if (User.HasClaim(c => (c.Type == "SuperAdmin" || c.Type == "Admin") && c.Value == "true"))
            {
                return Ok(true);
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return BadRequest("Invalid user ID format");
            }

            // Find module by route
            var modules = await _unitOfWork.Modules.FindAsync(m => m.Route == moduleRoute && m.IsActive);
            var module = modules.FirstOrDefault();
            if (module == null)
            {
                return NotFound($"Module with route '{moduleRoute}' not found");
            }

            // Get user roles
            var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(userId);

            foreach (var userRole in userRoles)
            {
                // Get role permissions
                var rolePermissions = await _unitOfWork.RolePermissions.GetByRoleIdAsync(userRole.RoleId);

                foreach (var rolePermission in rolePermissions)
                {
                    // Get permission details
                    var permission = await _unitOfWork.Permissions.GetByIdAsync(rolePermission.PermissionId);

                    // Check if this permission matches the requirement
                    if (permission.ModuleId == module.Id &&
                        permission.Type.ToString().Equals(permissionType, StringComparison.OrdinalIgnoreCase))
                    {
                        return Ok(true);
                    }
                }
            }

            // If we get here, no matching permission was found
            return Ok(false);
        }
    }
}