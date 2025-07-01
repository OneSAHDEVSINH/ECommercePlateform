using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Commands.Update
{
    public class UpdateModuleHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateModuleCommand, AppResult<ModuleDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<ModuleDto>> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get module by ID
                var module = await _unitOfWork.Modules.GetByIdAsync(request.Id);
                if (module == null)
                    return AppResult<ModuleDto>.Failure($"Module with ID {request.Id} not found.");

                // Validate uniqueness if it is being updated
                if ((!string.IsNullOrEmpty(request.Name) && request.Name != module.Name) ||
                    (!string.IsNullOrEmpty(request.Route) && request.Route != module.Route) ||
                    (!string.IsNullOrEmpty(request.Icon) && request.Icon != module.Icon) ||
                    request.DisplayOrder != module.DisplayOrder)
                {
                    var nameResult = await _unitOfWork.Modules.EnsureNameRouteIconDPAreUniqueAsync(
                        request.Name!,
                        request.Route!,
                        request.Icon!,
                        request.DisplayOrder ?? module.DisplayOrder,
                        request.Id);
                    if (nameResult.IsFailure)
                        return AppResult<ModuleDto>.Failure(nameResult.Error);
                }

                // Update module properties
                module.Update(
                    request.Name ?? module.Name ?? string.Empty,
                    request.Description ?? module.Description ?? string.Empty,
                    request.Route ?? module.Route ?? string.Empty,
                    request.Icon ?? module.Icon ?? string.Empty,
                    request.DisplayOrder ?? module.DisplayOrder
                );

                if (request.IsActive.HasValue)
                    module.SetActive(request.IsActive.Value);

                // Update module in database
                await _unitOfWork.Modules.UpdateAsync(module);

                // Map to DTO and return
                var moduleDto = (ModuleDto)module;

                return AppResult<ModuleDto>.Success(moduleDto);
            }
            catch (Exception ex)
            {
                return AppResult<ModuleDto>.Failure($"An error occurred while updating the module: {ex.Message}");
            }
        }
    }
}