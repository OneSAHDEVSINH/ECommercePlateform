using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Enums;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Queries.GetRoleWithPermissions
{
    public class GetRoleWithPermissionsHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRoleWithPermissionsQuery, AppResult<RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<RoleDto>> Handle(GetRoleWithPermissionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetRoleWithPermissionsAsync(request.Id);
                if (role == null)
                    return AppResult<RoleDto>.Failure($"Role with ID {request.Id} not found.");

                var rolePermissions = await _unitOfWork.RolePermissions.GetByRoleIdAsync(role.Id);
                var allPermissions = await _unitOfWork.Permissions.GetAllAsync();

                var permissionsByModule = new Dictionary<Guid, List<RolePermissionDto>>();

                foreach (var rp in rolePermissions)
                {
                    var perm = allPermissions.FirstOrDefault(p => p.Id == rp.PermissionId);
                    if (perm == null) continue;

                    var moduleId = perm.ModuleId;
                    var permDto = new RolePermissionDto
                    {
                        Id = rp.Id,
                        RoleId = rp.RoleId,
                        PermissionId = rp.PermissionId,
                        PermissionType = perm.Type,
                        ModuleId = moduleId,
                        ModuleName = perm.Module?.Name ?? string.Empty
                    };

                    if (!permissionsByModule.TryGetValue(moduleId, out var modulePerms))
                    {
                        modulePerms = new List<RolePermissionDto>();
                        permissionsByModule[moduleId] = modulePerms;
                    }

                    modulePerms.Add(permDto);
                }

                var allPermissionsFlat = rolePermissions.Select(rp =>
                {
                    var perm = allPermissions.FirstOrDefault(p => p.Id == rp.PermissionId);
                    return new RolePermissionDto
                    {
                        Id = rp.Id,
                        RoleId = rp.RoleId,
                        PermissionId = rp.PermissionId,
                        PermissionType = perm?.Type ?? PermissionType.View,
                        ModuleId = perm?.ModuleId ?? Guid.Empty,
                        ModuleName = perm?.Module?.Name ?? string.Empty
                    };
                }).ToList();

                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsActive = role.IsActive,
                    //CreatedBy = role.CreatedBy,
                    CreatedOn = role.CreatedOn,
                    //ModifiedBy = role.ModifiedBy,
                    //ModifiedOn = role.ModifiedOn,
                    Permissions = allPermissionsFlat
                };

                return AppResult<RoleDto>.Success(roleDto);
            }
            catch (Exception ex)
            {
                return AppResult<RoleDto>.Failure($"An error occurred while retrieving role with permissions: {ex.Message}");
            }
        }
    }
}