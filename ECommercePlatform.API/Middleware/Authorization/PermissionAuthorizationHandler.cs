using ECommercePlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ECommercePlatform.API.Middleware.Authorization
{
    public class PermissionRequirement(string module, string permission) : IAuthorizationRequirement
    {
        public string Module { get; } = module;
        public string Permission { get; } = permission;
    }

    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;

        public PermissionAuthorizationHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emailClaim = context.User.FindFirst(ClaimTypes.Email)?.Value;
            var isSuperAdmin = context.User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true");

            Console.WriteLine($"AUTH DEBUG: User ID: {userIdClaim}, Email: {emailClaim}, IsSuperAdmin: {isSuperAdmin}");
            Console.WriteLine($"AUTH DEBUG: Module: {requirement.Module}, Permission: {requirement.Permission}");

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                Console.WriteLine("AUTH DEBUG: No valid user ID found");
                context.Fail();
                return;
            }

            // Super admin bypass
            if (isSuperAdmin || emailClaim != null && emailClaim.Equals("admin@admin.com", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("AUTH DEBUG: Admin bypass activated");
                context.Succeed(requirement);
                return;
            }

            // Use the existing PermissionService
            var hasPermission = await _permissionService.UserHasPermissionAsync(
                userId,
                requirement.Module,
                requirement.Permission);

            Console.WriteLine($"AUTH DEBUG: Permission check result: {hasPermission}");

            if (hasPermission)
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}