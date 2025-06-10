using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Queries.GetRoleById
{
    public class GetRoleByIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
            if (role == null) return null!;

            var rolePermissions = await _unitOfWork.RolePermissions.GetByRoleIdAsync(role.Id);
            var allPermissions = await _unitOfWork.Permissions.GetAllAsync();

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