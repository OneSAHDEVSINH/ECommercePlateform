using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Commands.Delete
{
    public class DeleteUserHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteUserCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.Id);
                if (user == null)
                    return AppResult.Failure($"User with ID {request.Id} not found.");

                // First delete associated user roles
                await _unitOfWork.UserRoles.DeleteByUserIdAsync(request.Id);

                // Then delete the user
                await _unitOfWork.Users.DeleteAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred while deleting the user: {ex.Message}");
            }
        }
    }
}