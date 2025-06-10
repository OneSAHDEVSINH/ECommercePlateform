using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Commands.Create
{
    public class CreateUserHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new Domain.Entities.User();
            user = user.With(
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                password: request.Password,
                bio: null
            );
            user.IsActive = request.IsActive;
            await _unitOfWork.Users.AddAsync(user);

            foreach (var roleId in request.RoleIds)
            {
                var userRole = UserRole.Create(user.Id, roleId, "system");
                await _unitOfWork.UserRoles.AddAsync(userRole);
            }

            await _unitOfWork.SaveChangesAsync();
            return user.Id;
        }
    }
}