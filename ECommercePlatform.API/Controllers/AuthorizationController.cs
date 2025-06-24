using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommercePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthorizationController(IUnitOfWork unitOfWork, IPermissionService permissionService) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPermissionService _permissionService = permissionService;

        [HttpGet("check")]
        public async Task<IActionResult> CheckPermission([FromQuery] string moduleRoute, [FromQuery] string permissionType)
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            // Check for super admin
            if (User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true"))
            {
                return Ok(new { hasPermission = true, reason = "SuperAdmin" });
            }

            // Check direct permission claim
            var requiredPermission = $"{moduleRoute}:{permissionType}";
            if (User.HasClaim(c => c.Type == "Permission" && c.Value == requiredPermission))
            {
                return Ok(new { hasPermission = true, reason = "JWT Claim" });
            }

            // Find module by route
            var module = await _unitOfWork.Modules.GetByRouteAsync(moduleRoute);
            if (module == null)
            {
                return NotFound(new { message = $"Module with route '{moduleRoute}' not found" });
            }

            // Check permission through database
            var hasPermission = await _permissionService.UserHasPermissionAsync(
                userId,
                module.Name ?? moduleRoute,
                permissionType);

            return Ok(new
            {
                hasPermission,
                reason = hasPermission ? "Database Check" : "No Permission"
            });
        }

        [HttpGet("user-permissions")]
        public async Task<IActionResult> GetUserPermissions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                // If no user ID, just return JWT claims
                var claimPermissions = User.Claims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value)
                    .ToList();

                return Ok(new
                {
                    permissions = claimPermissions,
                    isAdmin = User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true"),
                    source = "JWT Claims Only"
                });
            }

            // Get permissions from database
            var dbPermissions = await _permissionService.GetUserPermissionsAsync(userId);

            // Also get JWT claim permissions for comparison
            var jwtPermissions = User.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            var isAdmin = User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true");

            return Ok(new
            {
                permissions = dbPermissions,
                jwtPermissions,
                isAdmin,
                source = "Database"
            });
        }

        [HttpGet("modules")]
        public async Task<IActionResult> GetUserAccessibleModules()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            // Get all modules
            var allModules = await _unitOfWork.Modules.GetActiveModulesAsync();

            // If super admin, return all modules
            if (User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true"))
            {
                return Ok(allModules.Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Route,
                    m.Icon,
                    m.DisplayOrder,
                    permissions = new
                    {
                        canView = true,
                        //canAdd = true,
                        //canEdit = true,
                        canAddEdit = true,
                        canDelete = true
                    }
                }));
            }

            // Get user permissions
            var userPermissions = await _permissionService.GetUserPermissionsAsync(userId);

            // Filter modules user has access to
            var accessibleModules = allModules
                .Where(m => userPermissions.Any(p => p.ModuleName == m.Name && p.CanView))
                .Select(m =>
                {
                    var permission = userPermissions.First(p => p.ModuleName == m.Name);
                    return new
                    {
                        m.Id,
                        m.Name,
                        m.Route,
                        m.Icon,
                        m.DisplayOrder,
                        permissions = new
                        {
                            canView = permission.CanView,
                            //canAdd = permission.CanAdd,
                            //canEdit = permission.CanEdit,
                            canAddEdit = permission.CanAddEdit,
                            canDelete = permission.CanDelete
                        }
                    };
                })
                .OrderBy(m => m.DisplayOrder);

            return Ok(accessibleModules);
        }

        [HttpGet("test-password-hash")]
        [AllowAnonymous]
        public IActionResult TestPasswordHash()
        {
            var user = new User { UserName = "test" };
            var passwordHasher = new PasswordHasher<User>();
            var hash = passwordHasher.HashPassword(user, "Admin@1234");
            return Ok(new { hash });
        }

        [HttpPost("reset-admin-password")]
        [AllowAnonymous] // Remove this in production!
        public async Task<IActionResult> ResetAdminPassword()
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync("admin@admin.com");
            if (user == null)
                return NotFound("Admin user not found");

            var token = await _unitOfWork.UserManager.GeneratePasswordResetTokenAsync(user);
            var result = await _unitOfWork.UserManager.ResetPasswordAsync(user, token, "Admin@1234");

            if (result.Succeeded)
                return Ok("Password reset successfully");

            return BadRequest(result.Errors);
        }
    }
}