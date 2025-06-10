using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IModuleRepository _moduleRepository;

        public PermissionAuthorizationHandler(
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IRoleRepository roleRepository,
            IRolePermissionRepository rolePermissionRepository,
            IPermissionRepository permissionRepository,
            IModuleRepository moduleRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _permissionRepository = permissionRepository;
            _moduleRepository = moduleRepository;
        }

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

            var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);
            var allRoles = await _roleRepository.GetAllAsync();
            var allRolePermissions = new System.Collections.Generic.List<Domain.Entities.RolePermission>();
            foreach (var ur in userRoles)
            {
                var rolePerms = await _rolePermissionRepository.GetByRoleIdAsync(ur.RoleId);
                allRolePermissions.AddRange(rolePerms);
            }
            var allPermissions = await _permissionRepository.GetAllAsync();
            var allModules = await _moduleRepository.GetAllAsync();

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