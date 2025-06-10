using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Queries.GetAllRoles
{
    public class GetAllRolesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _unitOfWork.Roles.GetAllAsync();
            var allPermissions = await _unitOfWork.Permissions.GetAllAsync();
            var result = new List<RoleDto>();

            foreach (var role in roles)
            {
                var rolePermissions = await _unitOfWork.RolePermissions.GetByRoleIdAsync(role.Id);
                var permissionsDto = rolePermissions.Select(rp =>
                {
                    var perm = allPermissions.FirstOrDefault(p => p.Id == rp.PermissionId);
                    return new RolePermissionDto
                    {
                        Id = rp.Id,
                        RoleId = rp.RoleId,
                        PermissionId = rp.PermissionId,
                        PermissionType = perm?.Type.ToString(),
                        ModuleId = perm?.ModuleId ?? default,
                        ModuleName = perm?.Module?.Name
                    };
                }).ToList();

                result.Add(new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsActive = role.IsActive,
                    Permissions = permissionsDto
                });
            }
            return result;
        }
    }
}