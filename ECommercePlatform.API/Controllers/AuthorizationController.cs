﻿using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IServices;
using ECommercePlatform.Application.Models;
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
        public IActionResult TestPasswordHash([FromQuery] string password)
        {
            if (string.IsNullOrEmpty(password))
                return BadRequest("Password is required");

            var user = new User { UserName = "test" };
            var passwordHasher = new PasswordHasher<User>();
            var hash = passwordHasher.HashPassword(user, password);
            return Ok(new { hash, password });
        }

        [HttpGet("verify-hash")]
        [AllowAnonymous]
        public IActionResult VerifyHash([FromQuery] string hash, [FromQuery] string password)
        {
            if (string.IsNullOrEmpty(hash) || string.IsNullOrEmpty(password))
                return BadRequest("Hash and password are required");

            var user = new User(); // Temporary user object just for verification
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, hash, password);

            return Ok(new
            {
                isCorrect = result == PasswordVerificationResult.Success,
                resultType = result.ToString()
            });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.NewPassword))
                return BadRequest("Email and new password are required");

            var user = await _unitOfWork.UserManager.FindByEmailAsync(request.Email);
            if (user == null)
                return NotFound($"User with email '{request.Email}' not found");

            var token = await _unitOfWork.UserManager.GeneratePasswordResetTokenAsync(user);
            var result = await _unitOfWork.UserManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (result.Succeeded)
                return Ok($"Password for user '{request.Email}' reset successfully");

            return BadRequest(result.Errors);
        }

        // Keep the existing method for backward compatibility but mark as obsolete
        [HttpPost("reset-admin-password")]
        [AllowAnonymous]
        [Obsolete("This method is deprecated. Use reset-password instead.")]
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