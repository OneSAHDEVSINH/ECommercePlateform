using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IServices;
using MediatR;

namespace ECommercePlatform.Application.Features.Users.Commands.AssignRolesToUser
{
    public class AssignRolesToUserHandler(IUnitOfWork unitOfWork, ISuperAdminService superAdminService) : IRequestHandler<AssignRolesToUserCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ISuperAdminService _superAdminService = superAdminService;
        public async Task<AppResult> Handle(AssignRolesToUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get user from UserManager
                var user = await _unitOfWork.UserManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                    return AppResult.Failure($"User with ID {request.UserId} not found.");

                // Check if this is a super admin user
                bool isSuperAdmin = _superAdminService.IsSuperAdminEmail(user.Email);

                // Prevent removing all roles from a super admin
                if (isSuperAdmin && request.RoleIds.Count == 0)
                    return AppResult.Failure($"Cannot remove all roles from a super admin user.");

                // Get current roles
                var currentRoles = await _unitOfWork.UserManager.GetRolesAsync(user);

                // Remove existing roles
                if (currentRoles.Any())
                {
                    await _unitOfWork.UserManager.RemoveFromRolesAsync(user, currentRoles);
                }

                // Add new roles
                foreach (var roleId in request.RoleIds)
                {
                    var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
                    if (role == null)
                        return AppResult.Failure($"Role with ID {roleId} not found.");

                    var result = await _unitOfWork.UserManager.AddToRoleAsync(user, role.Name!);
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        return AppResult.Failure($"Failed to assign role {role.Name}: {errors}");
                    }
                }

                // Update modification tracking
                user.ModifiedBy = request.ModifiedBy;
                user.ModifiedOn = request.ModifiedOn;
                await _unitOfWork.UserManager.UpdateAsync(user);

                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred while assigning roles: {ex.Message}");
            }
        }
    }
}