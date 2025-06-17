using ECommercePlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            if (User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true"))
            {
                return Ok(true);
            }

            // Check direct permission claim
            var requiredPermission = $"{moduleRoute}:{permissionType}";
            if (User.HasClaim(c => c.Type == "Permission" && c.Value == requiredPermission))
            {
                return Ok(true);
            }

            // Find module by route
            var modules = await _unitOfWork.Modules.FindAsync(m => m.Route == moduleRoute && m.IsActive);
            var module = modules.FirstOrDefault();
            if (module == null)
            {
                return NotFound($"Module with route '{moduleRoute}' not found");
            }

            // Get user roles
            var userRoles = await _unitOfWork.UserManager.GetRolesAsync(
                await _unitOfWork.UserManager.FindByIdAsync(userIdClaim.Value));

            foreach (var roleName in userRoles)
            {
                // Get role by name
                var role = await _unitOfWork.RoleManager.FindByNameAsync(roleName);
                if (role == null) continue;

                // Get role permissions
                var rolePermissions = await _unitOfWork.RolePermissions.GetByRoleIdAsync(Guid.Parse(role.Id));

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

        [HttpGet("user-permissions")]
        public IActionResult GetUserPermissions()
        {
            // Extract permission claims from the token
            var permissions = User.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            var isAdmin = User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true");

            return Ok(new { permissions, isAdmin });
        }
    }
}