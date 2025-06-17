using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Commands.Update
{
    public class UpdateUserHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserCommand, AppResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get user by ID
                var user = await _unitOfWork.Users.GetByIdAsync(request.Id);
                if (user == null)
                    return AppResult<UserDto>.Failure($"User with ID {request.Id} not found.");

                // Check email uniqueness if changing
                if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
                {
                    var emailResult = await _unitOfWork.Users.EnsureEmailIsUniqueAsync(request.Email, request.Id);
                    if (emailResult.IsFailure)
                        return AppResult<UserDto>.Failure(emailResult.Error);
                }

                // Update user properties (only if provided)
                user = user.With(
                    firstName: request.FirstName ?? user.FirstName,
                    lastName: request.LastName ?? user.LastName,
                    email: request.Email ?? user.Email,
                    password: request.Password ?? null, // null means don't change
                    gender: request.Gender ?? user.Gender,
                    dateOfBirth: request.DateOfBirth ?? user.DateOfBirth,
                    phoneNumber: request.PhoneNumber ?? user.PhoneNumber,
                    bio: request.Bio ?? user.Bio
                );

                if (request.IsActive.HasValue)
                    user.IsActive = request.IsActive.Value;

                user.ModifiedBy = request.ModifiedBy;
                user.ModifiedOn = request.ModifiedOn;

                await _unitOfWork.Users.UpdateAsync(user);

                // Update roles if provided
                if (request.RoleIds != null && request.RoleIds.Any())
                {
                    // Remove existing roles
                    await _unitOfWork.UserRoles.DeleteByUserIdAsync(user.Id);

                    // Add new roles
                    foreach (var roleId in request.RoleIds)
                    {
                        var userRole = Domain.Entities.UserRole.Create(user.Id, roleId);
                        userRole.CreatedBy = request.ModifiedBy;
                        userRole.CreatedOn = request.ModifiedOn;
                        await _unitOfWork.UserRoles.AddAsync(userRole);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                // Reload user to get updated details including roles
                var updatedUser = await _unitOfWork.Users.GetByIdAsync(user.Id);
                return updatedUser != null
                    ? AppResult<UserDto>.Success((UserDto)updatedUser)
                    : AppResult<UserDto>.Failure("User was updated but could not be retrieved.");
            }
            catch (Exception ex)
            {
                return AppResult<UserDto>.Failure($"An error occurred while updating the user: {ex.Message}");
            }
        }
    }
}