using ECommercePlatform.Application.Common.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace ECommercePlatform.API.Middleware.Authorization
{
    public class IdentityPermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // Check if user is authenticated
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            // Super admin bypass - always has all permissions
            if (context.User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Check for specific permission in claims
            var requiredPermission = $"{requirement.Module}:{requirement.Permission}";

            if (context.User.HasClaim(c => c.Type == "Permission" && c.Value == requiredPermission))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // If no matching claim was found, let other handlers try
            // Don't call context.Fail() here to allow PermissionAuthorizationHandler to check database
            return Task.CompletedTask;
        }
    }
}