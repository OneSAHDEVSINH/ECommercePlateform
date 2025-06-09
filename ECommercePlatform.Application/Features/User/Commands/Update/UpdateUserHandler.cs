using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Commands.Update
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserHandler(IUserRepository userRepository, IUserRoleRepository userRoleRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null) return false;

            user = user.With(
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email
            );
            user.IsActive = request.IsActive;
            await _userRepository.UpdateAsync(user);

            // Remove old roles
            await _userRoleRepository.DeleteByUserIdAsync(user.Id);

            // Add new roles
            foreach (var roleId in request.RoleIds)
            {
                var userRole = UserRole.Create(user.Id, roleId, "system");
                await _userRoleRepository.AddAsync(userRole);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}