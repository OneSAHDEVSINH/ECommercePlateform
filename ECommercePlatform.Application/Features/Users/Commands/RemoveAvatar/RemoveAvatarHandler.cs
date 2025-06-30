using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Users.Commands.RemoveAvatar
{
    public class RemoveAvatarHandler : IRequestHandler<RemoveAvatarCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAvatarHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(RemoveAvatarCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
                if (user == null)
                    return AppResult.Failure("User not found.");

                // Remove avatar
                user.Avatar = null;
                user.ModifiedOn = DateTime.UtcNow;

                await _unitOfWork.Users.UpdateAsync(user);

                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred while removing avatar: {ex.Message}");
            }
        }
    }
}