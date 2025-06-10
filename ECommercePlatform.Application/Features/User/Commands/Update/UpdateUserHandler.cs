using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Commands.Update
{
    public class UpdateUserHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.Id);
            if (user == null) return false;

            user = user.With(
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email
            );
            user.IsActive = request.IsActive;
            await _unitOfWork.Users.UpdateAsync(user);

            // Remove old roles
            await _unitOfWork.UserRoles.DeleteByUserIdAsync(user.Id);

            // Add new roles
            foreach (var roleId in request.RoleIds)
            {
                var userRole = UserRole.Create(user.Id, roleId, "system");
                await _unitOfWork.UserRoles.AddAsync(userRole);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}