using ECommercePlatform.Application.Common.Authorization.Requirements;
using ECommercePlatform.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommercePlatform.API.Middleware.Authorization
{
    public class PermissionAuthorizationHandler(
        IServiceProvider serviceProvider,
        ILogger<PermissionAuthorizationHandler> logger) : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<PermissionAuthorizationHandler> _logger = logger;

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            try
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    _logger.LogWarning("No valid user ID found in claims");
                    context.Fail();
                    return;
                }

                // Super admin bypass
                if (context.User.IsInRole("SuperAdmin") ||
                    context.User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true"))
                {
                    _logger.LogInformation($"SuperAdmin bypass for user {userId}");
                    context.Succeed(requirement);
                    return;
                }

                // Create a scope for the permission service
                using var scope = _serviceProvider.CreateScope();
                var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

                // Check permission from database
                var hasPermission = await permissionService.UserHasPermissionAsync(
                    userId,
                    requirement.Module,
                    requirement.Permission);

                if (hasPermission)
                {
                    _logger.LogInformation(
                        $"Direct permission found for user {userId}: Module={requirement.Module}, Permission={requirement.Permission}");
                    context.Succeed(requirement);
                    return;
                }

                // Check for hierarchical permissions if View permission is requested
                if (requirement.Permission.Equals("View", StringComparison.OrdinalIgnoreCase))
                {
                    bool hasHierarchicalPermission = false;

                    // Check for cross-entity permissions
                    switch (requirement.Module.ToLower())
                    {
                        case "countries":
                            // Countries can be viewed if user has states or cities view permission
                            hasHierarchicalPermission =
                                await permissionService.UserHasPermissionAsync(userId, "States", "View") ||
                                await permissionService.UserHasPermissionAsync(userId, "Cities", "View");
                            break;


                        case "states":
                            // States can be viewed if user has cities view permission
                            hasHierarchicalPermission =
                                await permissionService.UserHasPermissionAsync(userId, "Cities", "View");
                            break;

                        case "roles":
                            // Roles can be viewed if user has users view permission
                            hasHierarchicalPermission =
                                await permissionService.UserHasPermissionAsync(userId, "Users", "View");
                            break;

                        case "modules":
                            // Modules can be viewed if user has roles view permission
                            hasHierarchicalPermission =
                                await permissionService.UserHasPermissionAsync(userId, "Roles", "View");
                            break;
                    }

                    if (hasHierarchicalPermission)
                    {
                        _logger.LogInformation(
                            $"Hierarchical permission found for user {userId}: Module={requirement.Module}, Permission={requirement.Permission}");
                        context.Succeed(requirement);
                        return;
                    }
                }

                _logger.LogInformation(
            $"No permission found for user {userId}: Module={requirement.Module}, Permission={requirement.Permission}");
                context.Fail();

                _logger.LogInformation(
                    $"Permission check for user {userId}: Module={requirement.Module}, " +
                    $"Permission={requirement.Permission}, Result={hasPermission}");

                if (hasPermission)
                    context.Succeed(requirement);
                else
                    context.Fail();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permissions");
                context.Fail();
            }
        }
    }
}