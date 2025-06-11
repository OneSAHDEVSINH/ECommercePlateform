using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Commands.Delete
{
    public class DeleteModuleHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteModuleCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var module = await _unitOfWork.Modules.GetByIdAsync(request.Id);
                if (module == null)
                    return AppResult.Failure($"Module with ID {request.Id} not found.");

                // Check if module has permissions
                if (module.Permissions != null && module.Permissions.Any())
                {
                    return AppResult.Failure("Cannot delete module with associated permissions. Please delete the permissions first.");
                }

                // Delete the module
                await _unitOfWork.Modules.DeleteAsync(module);
                await _unitOfWork.SaveChangesAsync();

                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred while deleting the module: {ex.Message}");
            }
        }
    }
}