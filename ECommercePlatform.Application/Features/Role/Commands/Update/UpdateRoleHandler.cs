using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Commands.Update
{
    public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, bool>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRoleHandler(IRoleRepository roleRepository, IPermissionRepository permissionRepository, IRolePermissionRepository rolePermissionRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id);
            if (role == null) return false;

            role = Domain.Entities.Role.Create(request.Name, request.Description, "system");
            role.IsActive = request.IsActive;
            role.Id = request.Id;
            await _roleRepository.UpdateAsync(role);

            // Remove old permissions
            await _rolePermissionRepository.DeleteByRoleIdAsync(role.Id);

            // Add new permissions
            foreach (var perm in request.Permissions)
            {
                var permission = await _permissionRepository.GetByModuleAndTypeAsync(perm.ModuleId, perm.PermissionType);
                if (permission != null)
                {
                    var rolePermission = RolePermission.Create(role.Id, permission.Id, "system");
                    await _rolePermissionRepository.AddAsync(rolePermission);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}