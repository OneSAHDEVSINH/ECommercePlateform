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

                // Validate name uniqueness if name is being updated
                if (!string.IsNullOrEmpty(request.Name) && request.Name != module.Name)
                {
                    var nameResult = await _unitOfWork.Modules.EnsureNameIsUniqueAsync(request.Name, request.Id);
                    if (nameResult.IsFailure)
                        return AppResult<ModuleDto>.Failure(nameResult.Error);
                }

                // Validate route uniqueness if route is being updated
                if (!string.IsNullOrEmpty(request.Route) && request.Route != module.Route)
                {
                    var routeResult = await _unitOfWork.Modules.EnsureRouteIsUniqueAsync(request.Route, request.Id);
                    if (routeResult.IsFailure)
                        return AppResult<ModuleDto>.Failure(routeResult.Error);
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
                    module.IsActive = request.IsActive.Value;

                module.ModifiedBy = request.ModifiedBy;
                module.ModifiedOn = request.ModifiedOn;

                // Update module in database
                await _unitOfWork.Modules.UpdateAsync(module);
                await _unitOfWork.SaveChangesAsync();

                // Reload the module to ensure we have updated data
                var updatedModule = await _unitOfWork.Modules.GetByIdAsync(module.Id);
                if (updatedModule == null)
                    return AppResult<ModuleDto>.Failure("Module was updated but could not be retrieved.");

                // Map to DTO and return
                var moduleDto = new ModuleDto
                {
                    Id = updatedModule.Id,
                    Name = updatedModule.Name,
                    Description = updatedModule.Description,
                    Route = updatedModule.Route,
                    Icon = updatedModule.Icon,
                    DisplayOrder = updatedModule.DisplayOrder,
                    IsActive = updatedModule.IsActive,
                    CreatedOn = updatedModule.CreatedOn,
                    Permissions = updatedModule.Permissions?.Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Type = p.Type,
                        ModuleId = p.ModuleId,
                        ModuleName = updatedModule.Name,
                        ModuleRoute = updatedModule.Route,
                        IsActive = p.IsActive,
                        CreatedOn = p.CreatedOn
                    }).ToList() ?? new List<PermissionDto>()
                };

                return AppResult<ModuleDto>.Success(moduleDto);
            }
            catch (Exception ex)
            {
                return AppResult<ModuleDto>.Failure($"An error occurred while updating the module: {ex.Message}");
            }
        }
    }
}