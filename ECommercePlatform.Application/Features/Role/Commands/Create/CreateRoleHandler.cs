using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Commands.Create
{
    public class CreateRoleHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateRoleCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = Domain.Entities.Role.Create(request.Name, request.Description, "system");
            role.IsActive = request.IsActive;
            await _unitOfWork.Roles.AddAsync(role);

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
            return role.Id;
        }
    }
}