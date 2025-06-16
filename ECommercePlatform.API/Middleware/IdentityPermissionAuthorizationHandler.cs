using ECommercePlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ECommercePlatform.API.Middleware
{
    public class IdentityPermissionAuthorizationHandler() : AuthorizationHandler<PermissionRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // Check if user is authenticated
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            // Super admin bypass - always has all permissions
            if (context.User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true"))
            {
                context.Succeed(requirement);
                return;
            }

            // Check for specific permission in claims
            var requiredPermission = $"{requirement.Module}:{requirement.Permission}";
            
            if (context.User.HasClaim(c => c.Type == "Permission" && c.Value == requiredPermission))
            {
                context.Succeed(requirement);
                return;
            }
            
            // If no matching claim was found, the authorization fails
            context.Fail();
        }
    }
} 