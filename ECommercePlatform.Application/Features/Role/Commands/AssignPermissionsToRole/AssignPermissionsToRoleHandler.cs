using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Commands.AssignPermissionsToRole
{
    public class AssignPermissionsToRoleHandler(IUnitOfWork unitOfWork) : IRequestHandler<AssignPermissionsToRoleCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(AssignPermissionsToRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if role exists
                var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
                if (role == null)
                    return AppResult.Failure($"Role with ID {request.RoleId} not found.");

                // Delete existing role permissions
                await _unitOfWork.RolePermissions.DeleteByRoleIdAsync(role.Id);

                // Add new permissions
                foreach (var permGroup in request.Permissions)
                {
                    foreach (var permType in permGroup.PermissionTypes)
                    {
                        // Get the permission by module and type
                        var permission = await _unitOfWork.Permissions.GetByModuleAndTypeAsync(permGroup.ModuleId, permType);
                        if (permission == null) continue;

                        // Create role permission assignment
                        var rolePermission = RolePermission.Create(role.Id, permission.Id);
                        rolePermission.CreatedBy = request.ModifiedBy ?? "System";
                        rolePermission.CreatedOn = request.ModifiedOn;
                        await _unitOfWork.RolePermissions.AddAsync(rolePermission);
                    }
                }

                // Update role's audit info
                role.ModifiedBy = request.ModifiedBy ?? "System";
                role.ModifiedOn = request.ModifiedOn;
                await _unitOfWork.Roles.UpdateAsync(role);

                await _unitOfWork.SaveChangesAsync();
                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred while assigning permissions to role: {ex.Message}");
            }
        }
    }
}