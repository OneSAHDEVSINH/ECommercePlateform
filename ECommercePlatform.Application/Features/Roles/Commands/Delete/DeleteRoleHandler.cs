using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Roles.Commands.Delete
{
    public class DeleteRoleHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteRoleCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(request.Id);
                if (role == null)
                    return AppResult.Failure($"Role with ID {request.Id} not found.");

                // Check if role is used by users
                var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(role.Id);
                if (userRoles.Count != 0)
                    return AppResult.Failure("Cannot delete role. It is currently assigned to one or more users.");

                // First delete all role permissions
                await _unitOfWork.RolePermissions.DeleteByRoleIdAsync(role.Id);

                // Then delete the role itself
                await _unitOfWork.Roles.DeleteAsync(role);

                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred while deleting the role: {ex.Message}");
            }
        }
    }
}