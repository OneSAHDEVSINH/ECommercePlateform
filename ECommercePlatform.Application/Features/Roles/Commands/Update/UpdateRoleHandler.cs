using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Roles.Commands.Update
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
                if (request.Permissions != null)
                {
                    // Remove old permissions
                    await _unitOfWork.RolePermissions.DeleteByRoleIdAsync(role.Id);

                    // Add new permissions
                    foreach (var perm in request.Permissions)
                    {
                        // Verify module exists
                        var module = await _unitOfWork.Modules.GetByIdAsync(perm.ModuleId);
                        if (module == null) continue;

                        var rolePermission = RolePermission.Create(
                            role.Id,
                            perm.ModuleId,
                            perm.CanView,
                            perm.CanAddEdit,
                            perm.CanDelete
                        );

                        rolePermission.SetCreatedBy(request.ModifiedBy ?? "system");
                        await _unitOfWork.RolePermissions.AddAsync(rolePermission);
                    }
                }

                //await _unitOfWork.SaveChangesAsync();

                // Reload the role with updated permissions for return
                var updatedRole = await _unitOfWork.Roles.GetRoleWithPermissionsAsync(role.Id);
                if (updatedRole == null)
                    return AppResult<RoleDto>.Failure("Role was updated but could not be retrieved.");

                // Map to DTO using explicit operator
                var roleDto = (RoleDto)updatedRole;

                return AppResult<RoleDto>.Success(roleDto);
            }
            catch (Exception ex)
            {
                return AppResult<RoleDto>.Failure($"An error occurred while updating the role: {ex.Message}");
            }
        }
    }
}