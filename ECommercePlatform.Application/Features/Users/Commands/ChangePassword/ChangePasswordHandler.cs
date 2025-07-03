using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordHandler(IUnitOfWork unitOfWork) : IRequestHandler<ChangePasswordCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.UserManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                    return AppResult.Failure("User not found.");

                // Verify current password
                var passwordCheck = await _unitOfWork.UserManager.CheckPasswordAsync(user, request.CurrentPassword);
                if (!passwordCheck)
                    return AppResult.Failure("Current password is incorrect.");

                // Change password
                var result = await _unitOfWork.UserManager.ChangePasswordAsync(
                    user,
                    request.CurrentPassword,
                    request.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return AppResult.Failure($"Failed to change password: {errors}");
                }

                await _unitOfWork.UserManager.UpdateAsync(user);

                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred while changing password: {ex.Message}");
            }
        }
    }
}