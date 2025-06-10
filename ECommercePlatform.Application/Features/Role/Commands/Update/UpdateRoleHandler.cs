using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Commands.Update
{
    public class UpdateRoleHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateRoleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(request.Id);
            if (role == null) return false;

            role = Domain.Entities.Role.Create(request.Name, request.Description, "system");
            role.IsActive = request.IsActive;
            role.Id = request.Id;
            await _unitOfWork.Roles.UpdateAsync(role);

            // Remove old permissions
            await _unitOfWork.RolePermissions.DeleteByRoleIdAsync(role.Id);

            // Add new permissions
            foreach (var perm in request.Permissions)
            {
                var permission = await _unitOfWork.Permissions.GetByModuleAndTypeAsync(perm.ModuleId, perm.PermissionType);
                if (permission != null)
                {
                    var rolePermission = RolePermission.Create(role.Id, permission.Id, "system");
                    await _unitOfWork.RolePermissions.AddAsync(rolePermission);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}