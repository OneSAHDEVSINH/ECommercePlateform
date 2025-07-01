using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IServices;
using MediatR;

namespace ECommercePlatform.Application.Features.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileHandler(IUnitOfWork unitOfWork, IPermissionService permissionService) : IRequestHandler<UpdateUserProfileCommand, AppResult<UserProfileDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPermissionService _permissionService = permissionService;

        public async Task<AppResult<UserProfileDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.UserManager.FindByIdAsync(request.Id.ToString());
                if (user == null)
                    return AppResult<UserProfileDto>.Failure("User not found.");

                // Update only provided fields
                if (!string.IsNullOrEmpty(request.FirstName))
                    user.FirstName = request.FirstName;
                if (!string.IsNullOrEmpty(request.LastName))
                    user.LastName = request.LastName;
                if (request.Gender.HasValue)
                    user.Gender = request.Gender.Value;
                if (request.DateOfBirth.HasValue)
                    user.DateOfBirth = request.DateOfBirth.Value;
                if (request.PhoneNumber != null)
                    user.PhoneNumber = request.PhoneNumber;
                if (request.Bio != null)
                    user.Bio = request.Bio;

                var result = await _unitOfWork.UserManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return AppResult<UserProfileDto>.Failure($"Failed to update profile: {errors}");
                }

                // Reload user data
                var updatedUser = await _unitOfWork.Users.GetByIdAsync(user.Id);
                if (updatedUser == null)
                    return AppResult<UserProfileDto>.Failure("User was updated but could not be retrieved.");

                // Get permissions
                var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);

                // Get user roles
                var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(user.Id);
                var allRoles = await _unitOfWork.Roles.GetAllAsync();

                var rolesDto = userRoles
                    .Select(ur => allRoles.FirstOrDefault(r => r.Id == ur.RoleId))
                    .Where(r => r != null)
                    .Select(r => (RoleDto)r!)
                    .ToList();

                var userProfile = UserProfileDto.Create(user, rolesDto, permissions);

                return AppResult<UserProfileDto>.Success(userProfile);
            }
            catch (Exception ex)
            {
                return AppResult<UserProfileDto>.Failure($"An error occurred while updating profile: {ex.Message}");
            }
        }
    }
}