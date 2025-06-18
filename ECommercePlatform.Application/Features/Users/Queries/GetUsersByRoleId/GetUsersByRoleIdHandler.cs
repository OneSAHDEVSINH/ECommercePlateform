using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Users.Queries.GetUsersByRoleId
{
    public class GetUsersByRoleIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUsersByRoleIdQuery, AppResult<List<UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<UserDto>>> Handle(GetUsersByRoleIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // First check if role exists
                var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
                if (role == null)
                    return AppResult<List<UserDto>>.Failure($"Role with ID {request.RoleId} not found.");

                // Get all users with this role
                var users = await _unitOfWork.Users.GetUsersByRoleIdAsync(request.RoleId);

                // Filter by active if requested
                if (request.ActiveOnly)
                    users = users.Where(u => u.IsActive && !u.IsDeleted).ToList();

                // Map users to DTOs with their roles
                var userDtos = new List<UserDto>();
                foreach (var user in users)
                {
                    // Get user roles
                    var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(user.Id);
                    var allRoles = await _unitOfWork.Roles.GetAllAsync();

                    var rolesDto = userRoles.Select(ur =>
                    {
                        var userRole = allRoles.FirstOrDefault(r => r.Id == ur.RoleId);
                        return userRole != null ? (RoleDto)userRole : null;
                    })
                    .Where(r => r != null)
                    .Cast<RoleDto>() // Ensure non-nullable type
                    .ToList();

                    // Create user DTO with roles
                    var userDto = new UserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Password = user.PasswordHash,
                        PhoneNumber = user.PhoneNumber,
                        Gender = user.Gender,
                        DateOfBirth = user.DateOfBirth,
                        Bio = user.Bio,
                        IsActive = user.IsActive,
                        CreatedOn = user.CreatedOn,
                        Roles = rolesDto
                    };

                    userDtos.Add(userDto);
                }

                return AppResult<List<UserDto>>.Success(userDtos);
            }
            catch (Exception ex)
            {
                return AppResult<List<UserDto>>.Failure($"An error occurred while retrieving users by role: {ex.Message}");
            }
        }
    }
}