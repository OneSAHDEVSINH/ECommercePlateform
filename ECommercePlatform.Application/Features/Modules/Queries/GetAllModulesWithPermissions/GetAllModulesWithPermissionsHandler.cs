using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetAllModulesWithPermissions
{
    public class GetAllModulesWithPermissionsHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllModulesWithPermissionsQuery, AppResult<List<ModuleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<ModuleDto>>> Handle(GetAllModulesWithPermissionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get all modules with their permissions
                var modules = request.ActiveOnly
                    ? await _unitOfWork.Modules.GetActiveModulesAsync()
                    : await _unitOfWork.Modules.GetAllAsync();

                // Map to DTOs
                var moduleDtos = new List<ModuleDto>();

                foreach (var module in modules)
                {
                    // Get permissions for this module
                    var permissions = await _unitOfWork.Permissions.GetByModuleIdAsync(module.Id);

                    moduleDtos.Add(new ModuleDto
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
                            //Name = p.Name,
                            //Description = p.Description,
                            Type = p.Type,
                            ModuleId = p.ModuleId,
                            ModuleName = module.Name,
                            ModuleRoute = module.Route,
                            IsActive = p.IsActive,
                            CreatedOn = p.CreatedOn
                        }).ToList()
                    });
                }

                // Sort by display order
                moduleDtos = moduleDtos.OrderBy(m => m.DisplayOrder).ToList();

                return AppResult<List<ModuleDto>>.Success(moduleDtos);
            }
            catch (Exception ex)
            {
                return AppResult<List<ModuleDto>>.Failure($"An error occurred while retrieving modules with permissions: {ex.Message}");
            }
        }
    }
}