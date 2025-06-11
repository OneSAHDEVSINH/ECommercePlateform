using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Commands.AssignRoles
{
    public class AssignRolesToUserHandler(IUnitOfWork unitOfWork) : IRequestHandler<AssignRolesToUserCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(AssignRolesToUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if user exists
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
                if (user == null)
                    return AppResult.Failure($"User with ID {request.UserId} not found.");

                // Remove existing roles
                await _unitOfWork.UserRoles.DeleteByUserIdAsync(request.UserId);

                // Add new roles
                foreach (var roleId in request.RoleIds)
                {
                    // Verify role exists
                    var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
                    if (role == null)
                        return AppResult.Failure($"Role with ID {roleId} not found.");

                    var userRole = Domain.Entities.UserRole.Create(request.UserId, roleId);
                    userRole.CreatedBy = request.ModifiedBy;
                    userRole.CreatedOn = request.ModifiedOn;
                    await _unitOfWork.UserRoles.AddAsync(userRole);
                }

                // Update user's modification tracking
                user.ModifiedBy = request.ModifiedBy;
                user.ModifiedOn = request.ModifiedOn;
                await _unitOfWork.Users.UpdateAsync(user);

                await _unitOfWork.SaveChangesAsync();

                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred while assigning roles: {ex.Message}");
            }
        }
    }
}