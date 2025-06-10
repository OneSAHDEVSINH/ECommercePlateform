using ECommercePlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ECommercePlatform.API.Middleware
{
    public class PermissionRequirement(string module, string permission) : IAuthorizationRequirement
    {
        public string Module { get; } = module;
        public string Permission { get; } = permission;
    }

    public class PermissionAuthorizationHandler(
        IUnitOfWork unitOfWork) : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
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
            if (isSuperAdmin || (emailClaim != null && emailClaim.ToLower() == "admin@admin.com"))
            {
                Console.WriteLine("AUTH DEBUG: Admin bypass activated");
                context.Succeed(requirement);
                return;
            }

            var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(userId);
            var allRoles = await _unitOfWork.Roles.GetAllAsync();
            var allRolePermissions = new System.Collections.Generic.List<Domain.Entities.RolePermission>();
            foreach (var ur in userRoles)
            {
                var rolePerms = await _unitOfWork.RolePermissions.GetByRoleIdAsync(ur.RoleId);
                allRolePermissions.AddRange(rolePerms);
            }
            var allPermissions = await _unitOfWork.Permissions.GetAllAsync();
            var allModules = await _unitOfWork.Modules.GetAllAsync();

            var module = allModules.FirstOrDefault(m => m.Name.Equals(requirement.Module, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                context.Fail();
                return;
            }

            var permission = allPermissions.FirstOrDefault(p => p.ModuleId == module.Id && p.Type.ToString().Equals(requirement.Permission, StringComparison.OrdinalIgnoreCase));
            if (permission == null)
            {
                context.Fail();
                return;
            }

            var hasPermission = allRolePermissions.Any(rp => rp.PermissionId == permission.Id);
            if (hasPermission)
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}