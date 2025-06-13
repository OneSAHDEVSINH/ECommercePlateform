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
            if (isSuperAdmin || (emailClaim != null && emailClaim.Equals("admin@admin.com", StringComparison.CurrentCultureIgnoreCase)))
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

/*
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ECommercePlatform.Application.Interfaces;

namespace ECommercePlatform.API.Middleware
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Module { get; }
        public string Permission { get; }

        public PermissionRequirement(string module, string permission)
        {
            Module = module;
            Permission = permission;
        }
    }

    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PermissionAuthorizationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            PermissionRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                return;
            }

            // Super admin bypass - always has all permissions
            if (context.User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true"))
            {
                context.Succeed(requirement);
                return;
            }

            // Get user ID
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return;
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return;
            }

            // Get user roles
            var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(userId);
            
            foreach (var userRole in userRoles)
            {
                // Get role permissions
                var rolePermissions = await _unitOfWork.RolePermissions.GetByRoleIdAsync(userRole.RoleId);
                
                foreach (var rolePermission in rolePermissions)
                {
                    // Get permission details
                    var permission = await _unitOfWork.Permissions.GetByIdAsync(rolePermission.PermissionId);
                    
                    // Get module details
                    var module = await _unitOfWork.Modules.GetByIdAsync(permission.ModuleId);
                    
                    // Check if this permission matches the requirement
                    if (module.Name == requirement.Module && 
                        permission.Type.ToString() == requirement.Permission)
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
            }
        }
    }

    // Admin bypass handler for super admins
    public class AdminBypassHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            // Check if there is a super admin claim
            if (context.User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true"))
            {
                // Mark all requirements as succeeded
                foreach (var requirement in context.PendingRequirements)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}*/