using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Users.Queries.GetCurrentUserProfile
{
    public class GetCurrentUserProfileHandler(IUnitOfWork unitOfWork, IPermissionService permissionService) : IRequestHandler<GetCurrentUserProfileQuery, AppResult<UserProfileDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPermissionService _permissionService = permissionService;

        public async Task<AppResult<UserProfileDto>> Handle(GetCurrentUserProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (!Guid.TryParse(request.UserId, out var userId))
                    return AppResult<UserProfileDto>.Failure("Invalid user ID format.");

                // Get user with roles
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    return AppResult<UserProfileDto>.Failure("User not found.");

                // Get user permissions
                var permissions = await _permissionService.GetUserPermissionsAsync(userId);

                // Get user roles
                var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(userId);
                var allRoles = await _unitOfWork.Roles.GetAllAsync();

                var rolesDto = userRoles
                    .Select(ur => allRoles.FirstOrDefault(r => r.Id == ur.RoleId))
                    .Where(r => r != null)
                    .Select(r => (RoleDto)r!)
                    .ToList();

                // Create UserProfileDto with all properties in the initializer
                //var userProfile = new UserProfileDto
                //{
                //    Id = user.Id,
                //    FirstName = user.FirstName,
                //    LastName = user.LastName,
                //    Email = user.Email,
                //    PhoneNumber = user.PhoneNumber,
                //    Gender = user.Gender,
                //    DateOfBirth = user.DateOfBirth,
                //    Bio = user.Bio,
                //    IsActive = user.IsActive,
                //    CreatedOn = user.CreatedOn,
                //    Roles = rolesDto,
                //    Permissions = permissions // Set in initializer
                //};

                var userProfile = UserProfileDto.Create(user, rolesDto, permissions);

                return AppResult<UserProfileDto>.Success(userProfile);
            }
            catch (Exception ex)
            {
                return AppResult<UserProfileDto>.Failure($"An error occurred while retrieving user profile: {ex.Message}");
            }
        }
    }
}