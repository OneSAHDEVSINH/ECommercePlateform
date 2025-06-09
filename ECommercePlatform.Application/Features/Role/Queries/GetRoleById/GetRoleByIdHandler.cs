using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Queries.GetRoleById
{
    public class GetRoleByIdHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IPermissionRepository _permissionRepository;

        public GetRoleByIdHandler(IRoleRepository roleRepository, IRolePermissionRepository rolePermissionRepository, IPermissionRepository permissionRepository)
        {
            _roleRepository = roleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId);
            if (role == null) return null!;

            var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(role.Id);
            var allPermissions = await _permissionRepository.GetAllAsync();

            var permissionsDto = rolePermissions.Select(rp =>
            {
                var perm = allPermissions.FirstOrDefault(p => p.Id == rp.PermissionId);
                return new RolePermissionDto
                {
                    Id = rp.Id,
                    RoleId = rp.RoleId,
                    PermissionId = rp.PermissionId,
                    PermissionType = perm?.Type.ToString() ?? string.Empty,
                    ModuleId = perm?.ModuleId ?? Guid.Empty,
                    ModuleName = perm?.Module?.Name ?? string.Empty
                };
            }).ToList();

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                Permissions = permissionsDto
            };
        }
    }
}