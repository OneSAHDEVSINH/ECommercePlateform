using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Commands.Create
{
    public class CreateModuleHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateModuleCommand, AppResult<ModuleDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<ModuleDto>> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate name uniqueness
                var nameResult = await _unitOfWork.Modules.EnsureNameIsUniqueAsync(request.Name);
                if (nameResult.IsFailure)
                    return AppResult<ModuleDto>.Failure(nameResult.Error);

                // Validate route uniqueness
                var routeResult = await _unitOfWork.Modules.EnsureRouteIsUniqueAsync(request.Route);
                if (routeResult.IsFailure)
                    return AppResult<ModuleDto>.Failure(routeResult.Error);

                // Create module entity
                var module = Domain.Entities.Module.Create(
                    request.Name,
                    request.Description ?? string.Empty,
                    request.Route,
                    request.Icon ?? string.Empty,
                    request.DisplayOrder
                );

                // Use the SetActive method from BaseEntity
                if (!request.IsActive)
                {
                    module.SetActive(false, request.CreatedBy ?? "system");
                }

                // Set audit fields
                module.SetCreatedBy(request.CreatedBy ?? "system");

                // Add module to database
                await _unitOfWork.Modules.AddAsync(module);
                await _unitOfWork.SaveChangesAsync();

                // Map to DTO and return
                var moduleDto = (ModuleDto)module;

                return AppResult<ModuleDto>.Success(moduleDto);
            }
            catch (Exception ex)
            {
                return AppResult<ModuleDto>.Failure($"An error occurred while creating the module: {ex.Message}");
            }
        }
    }
}