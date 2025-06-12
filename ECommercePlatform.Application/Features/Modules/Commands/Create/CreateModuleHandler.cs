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

                module.IsActive = request.IsActive;
                module.CreatedBy = request.CreatedBy;
                module.CreatedOn = request.CreatedOn;

                // Add module to database
                await _unitOfWork.Modules.AddAsync(module);
                await _unitOfWork.SaveChangesAsync();

                // Load the newly created module (with permissions, if any)
                var createdModule = await _unitOfWork.Modules.GetByIdAsync(module.Id);
                if (createdModule == null)
                    return AppResult<ModuleDto>.Failure("Module was created but could not be retrieved.");

                // Map to DTO and return
                var moduleDto = new ModuleDto
                {
                    Id = createdModule.Id,
                    Name = createdModule.Name,
                    Description = createdModule.Description,
                    Route = createdModule.Route,
                    Icon = createdModule.Icon,
                    DisplayOrder = createdModule.DisplayOrder,
                    IsActive = createdModule.IsActive,
                    CreatedOn = createdModule.CreatedOn,
                    Permissions = createdModule.Permissions?.Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        //Name = p.Name,
                        //Description = p.Description,
                        Type = p.Type,
                        ModuleId = p.ModuleId,
                        ModuleName = createdModule.Name,
                        ModuleRoute = createdModule.Route,
                        IsActive = p.IsActive,
                        CreatedOn = p.CreatedOn
                    }).ToList() ?? new List<PermissionDto>()
                };

                return AppResult<ModuleDto>.Success(moduleDto);
            }
            catch (Exception ex)
            {
                return AppResult<ModuleDto>.Failure($"An error occurred while creating the module: {ex.Message}");
            }
        }
    }
}