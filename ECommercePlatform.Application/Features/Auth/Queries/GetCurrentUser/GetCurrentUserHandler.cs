using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.Auth.Queries.GetCurrentUser
{
    public class GetCurrentUserHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<GetCurrentUserQuery, AppResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<AppResult<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
                    return AppResult<UserDto>.Failure("User is not authenticated");

                var userId = Guid.Parse(_currentUserService.UserId); // Convert string UserId to Guid
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                    return AppResult<UserDto>.Failure("User not found");

                // Get user roles
                var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(userId);
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

                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = user.PasswordHash,
                    IsActive = user.IsActive,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    Bio = user.Bio,
                    CreatedOn = user.CreatedOn,
                    Roles = rolesDto
                };

                return AppResult<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                return AppResult<UserDto>.Failure($"An error occurred while getting the current user: {ex.Message}");
            }
        }
    }
}