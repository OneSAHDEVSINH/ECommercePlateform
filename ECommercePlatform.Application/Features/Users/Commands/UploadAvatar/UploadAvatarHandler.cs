using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Users.Commands.UploadAvatar
{
    public class UploadAvatarHandler : IRequestHandler<UploadAvatarCommand, AppResult<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UploadAvatarHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<AppResult<string>> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
                if (user == null)
                    return AppResult<string>.Failure("User not found.");

                // Convert file to byte array
                using var memoryStream = new MemoryStream();
                await request.File.CopyToAsync(memoryStream, cancellationToken);
                var avatarData = memoryStream.ToArray();

                // Update user avatar
                user.Avatar = avatarData;
                user.ModifiedOn = DateTime.UtcNow;

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // Return a base64 string or URL depending on your implementation
                var avatarBase64 = Convert.ToBase64String(avatarData);
                var avatarUrl = $"data:{request.File.ContentType};base64,{avatarBase64}";

                return AppResult<string>.Success(avatarUrl);
            }
            catch (Exception ex)
            {
                return AppResult<string>.Failure($"An error occurred while uploading avatar: {ex.Message}");
            }
        }
    }
}