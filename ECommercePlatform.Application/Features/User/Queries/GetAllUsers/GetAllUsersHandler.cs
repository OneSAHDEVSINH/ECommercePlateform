using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Queries.GetUserWithRoles
{
    public class GetUserWithRolesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUserWithRolesQuery, AppResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<UserDto>> Handle(GetUserWithRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get user by ID with eager loading of roles
                var user = await _unitOfWork.Users.GetByIdAsync(request.Id);
                if (user == null)
                    return AppResult<UserDto>.Failure($"User with ID {request.Id} not found.");

                // Get user roles explicitly to ensure fresh data
                var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(user.Id);
                var allRoles = await _unitOfWork.Roles.GetAllAsync();

                // Map roles to DTOs, filtering out nulls
                var rolesDto = userRoles.Select(ur =>
                {
                    var role = allRoles.FirstOrDefault(r => r.Id == ur.RoleId);
                    return role != null ? (RoleDto)role : null;
                })
                .Where(r => r != null)
                .Cast<RoleDto>() // Ensure non-nullable result
                .ToList();

                // Create UserDto using object initializer to properly handle init-only properties
                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = user.Password,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    Bio = user.Bio,
                    IsActive = user.IsActive,
                    CreatedOn = user.CreatedOn,
                    Roles = rolesDto // Assign roles here
                };

                return AppResult<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                return AppResult<UserDto>.Failure($"An error occurred while retrieving user with roles: {ex.Message}");
            }
        }
    }
}