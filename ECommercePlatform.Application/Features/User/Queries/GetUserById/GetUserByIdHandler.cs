using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Queries.GetUserById
{
    public class GetUserByIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null) return null;

            var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(user.Id);
            var allRoles = await _unitOfWork.Roles.GetAllAsync();
            var rolesDto = userRoles.Select(ur =>
            {
                var role = allRoles.FirstOrDefault(r => r.Id == ur.RoleId);
                return new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsActive = role.IsActive
                };
            }).ToList();

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = rolesDto
            };
        }
    }
}