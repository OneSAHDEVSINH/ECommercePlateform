using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Users.Queries.GetUserByEmail
{
    public class GetUserByEmailHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUserByEmailQuery, AppResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.FindUserByEmailAsync(request.Email);
                if (user == null)
                    return AppResult<UserDto>.Failure($"User with email {request.Email} not found.");

                // Get user roles
                var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(user.Id);
                var allRoles = await _unitOfWork.Roles.GetAllAsync();

                var rolesDto = userRoles.Select(ur =>
                {
                    var role = allRoles.FirstOrDefault(r => r.Id == ur.RoleId);
                    return role != null ? (RoleDto)role : null;
                })
                .Where(r => r != null)
                .Cast<RoleDto>() // Ensure non-nullable type
                .ToList();

                // Use object initializer to set init-only property
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
                return AppResult<UserDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}