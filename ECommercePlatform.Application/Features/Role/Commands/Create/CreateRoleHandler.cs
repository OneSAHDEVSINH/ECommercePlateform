using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Commands.Create
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Guid>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoleHandler(IRoleRepository roleRepository, IPermissionRepository permissionRepository, IRolePermissionRepository rolePermissionRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = Domain.Entities.Role.Create(request.Name, request.Description, "system");
            role.IsActive = request.IsActive;
            await _roleRepository.AddAsync(role);

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
            return role.Id;
        }
    }
}