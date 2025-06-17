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
                    // Check if user is Admin (super admin)
                    if (IsAdminUser(context.User))
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private bool IsAdminUser(ClaimsPrincipal user)
        {
            // Check for Admin role claim
            return user.IsInRole("Admin") || user.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        }
    }
}