using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Commands.Update
{
    public class UpdateRoleHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateRoleCommand, AppResult<RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(request.Id);
                if (role == null)
                    return AppResult<RoleDto>.Failure($"Role with ID {request.Id} not found.");

                // Validate role name uniqueness if name is being updated
                if (!string.IsNullOrEmpty(request.Name) && request.Name != role.Name)
                {
                    var nameResult = await _unitOfWork.Roles.EnsureNameIsUniqueAsync(request.Name, request.Id);
                    if (nameResult.IsFailure)
                        return AppResult<RoleDto>.Failure(nameResult.Error);

                    role.UpdateProperties(name: request.Name);
                }

                // Update other fields only if provided
                if (request.Description != null)
                    role.UpdateProperties(description: request.Description);

                if (request.IsActive.HasValue)
                    role.IsActive = request.IsActive.Value;

                role.ModifiedBy = request.ModifiedBy;
                role.ModifiedOn = request.ModifiedOn;

                await _unitOfWork.Roles.UpdateAsync(role);

                // Update permissions if provided
                if (request.Permissions != null && request.Permissions.Any())
                {
                    // Remove old permissions
                    await _unitOfWork.RolePermissions.DeleteByRoleIdAsync(role.Id);

                    // Add new permissions
                    foreach (var perm in request.Permissions)
                    {
                        // Parse permission type from string to enum
                        if (!Enum.TryParse<PermissionType>(perm.PermissionType, true, out var permissionTypeEnum))
                            continue; // Skip invalid permission types

                        var permission = await _unitOfWork.Permissions.GetByModuleAndTypeAsync(perm.ModuleId, perm.PermissionType);
                        if (permission != null)
                        {
                            var rolePermission = RolePermission.Create(role.Id, permission.Id);
                            rolePermission.CreatedBy = request.ModifiedBy;
                            rolePermission.CreatedOn = request.ModifiedOn;
                            await _unitOfWork.RolePermissions.AddAsync(rolePermission);
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                // Reload the role with updated permissions for return
                var updatedRole = await _unitOfWork.Roles.GetRoleWithPermissionsAsync(role.Id);
                if (updatedRole == null)
                    return AppResult<RoleDto>.Failure("Role was updated but could not be retrieved.");

                // Map Entity to DTO
                var rolePermissions = await _unitOfWork.RolePermissions.GetByRoleIdAsync(updatedRole.Id);
                var allPermissions = await _unitOfWork.Permissions.GetAllAsync();

                var permissionDtos = rolePermissions.Select(rp =>
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
                    Id = updatedRole.Id,
                    Name = updatedRole.Name,
                    Description = updatedRole.Description,
                    IsActive = updatedRole.IsActive,
                    CreatedOn = updatedRole.CreatedOn,
                    //CreatedBy = updatedRole.CreatedBy,
                    //ModifiedOn = updatedRole.ModifiedOn,
                    //ModifiedBy = updatedRole.ModifiedBy,
                    Permissions = permissionDtos
                };

                return AppResult<RoleDto>.Success(roleDto);
            }
            catch (Exception ex)
            {
                return AppResult<RoleDto>.Failure($"An error occurred while updating the role: {ex.Message}");
            }
        }
    }
}