using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Users.Queries.GetUserWithRoles
{
    public class GetUserWithRolesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUserWithRolesQuery, AppResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<UserDto>> Handle(GetUserWithRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Find user with full role details
                var user = await _unitOfWork.Users.GetByIdAsync(request.Id);
                if (user == null)
                    return AppResult<UserDto>.Failure($"User with ID {request.Id} not found.");

                // Get user roles with all needed relationships
                var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(user.Id);
                var allRoles = await _unitOfWork.Roles.GetAllAsync();

                // Map roles to DTOs with their permissions
                var rolesDto = userRoles.Select(ur =>
                {
                    var role = allRoles.FirstOrDefault(r => r.Id == ur.RoleId);
                    return role != null ? (RoleDto)role : null;
                })
                .Where(r => r != null)
                .Cast<RoleDto>() // Ensure non-nullable type
                .ToList();

                // Create UserDto using object initializer for init-only properties
                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    
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