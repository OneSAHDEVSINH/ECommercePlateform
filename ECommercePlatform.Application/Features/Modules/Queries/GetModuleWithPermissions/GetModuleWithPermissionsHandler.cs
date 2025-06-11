using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetModuleWithPermissions
{
    public class GetModuleWithPermissionsHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetModuleWithPermissionsQuery, AppResult<ModuleDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<ModuleDto>> Handle(GetModuleWithPermissionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var module = await _unitOfWork.Modules.GetByIdAsync(request.Id);
                if (module == null)
                    return AppResult<ModuleDto>.Failure($"Module with ID {request.Id} not found.");

                // Get permissions for this module
                var permissions = await _unitOfWork.Permissions.GetByModuleIdAsync(module.Id);

                // Map to DTO
                var moduleDto = new ModuleDto
                {
                    Id = module.Id,
                    Name = module.Name,
                    Description = module.Description,
                    Route = module.Route,
                    Icon = module.Icon,
                    DisplayOrder = module.DisplayOrder,
                    IsActive = module.IsActive,
                    CreatedOn = module.CreatedOn,
                    Permissions = permissions.Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Type = p.Type,
                        ModuleId = p.ModuleId,
                        ModuleName = module.Name,
                        ModuleRoute = module.Route,
                        IsActive = p.IsActive,
                        CreatedOn = p.CreatedOn
                    }).ToList()
                };

                return AppResult<ModuleDto>.Success(moduleDto);
            }
            catch (Exception ex)
            {
                return AppResult<ModuleDto>.Failure($"An error occurred while retrieving the module with permissions: {ex.Message}");
            }
        }
    }
}