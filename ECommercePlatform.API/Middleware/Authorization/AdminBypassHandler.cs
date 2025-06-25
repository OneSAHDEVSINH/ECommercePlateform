using ECommercePlatform.Application.Common.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ECommercePlatform.API.Middleware.Authorization
{
    public class AdminBypassHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            // Check if this is a permission requirement
            foreach (var requirement in context.PendingRequirements.ToList())
            {
                if (requirement is PermissionRequirement)
                {
                    // Check if user is SuperAdmin
                    if (IsSuperAdmin(context.User))
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private static bool IsSuperAdmin(ClaimsPrincipal user)
        {
            // Check for SuperAdmin role or claim
            return user.IsInRole("SuperAdmin") ||
                   user.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true");
        }
    }
}