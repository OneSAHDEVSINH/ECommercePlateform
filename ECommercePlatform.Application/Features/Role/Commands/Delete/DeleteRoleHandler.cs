using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Commands.Delete
{
    public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRoleHandler(IRoleRepository roleRepository, IRolePermissionRepository rolePermissionRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id);
            if (role == null) return false;

            await _rolePermissionRepository.DeleteByRoleIdAsync(role.Id);
            await _roleRepository.DeleteAsync(role);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}