using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetAllModules
{
    public class GetAllModulesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllModulesQuery, AppResult<List<ModuleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<ModuleDto>>> Handle(GetAllModulesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var modules = request.ActiveOnly
                    ? await _unitOfWork.Modules.GetActiveModulesAsync()
                    : await _unitOfWork.Modules.GetAllAsync();

                // Map to DTOs
                var moduleDtos = modules.Select(m => new ModuleDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Route = m.Route,
                    Icon = m.Icon,
                    DisplayOrder = m.DisplayOrder,
                    IsActive = m.IsActive,
                    CreatedOn = m.CreatedOn,
                    Permissions = m.Permissions?.Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Type = p.Type,
                        ModuleId = p.ModuleId,
                        ModuleName = m.Name,
                        ModuleRoute = m.Route,
                        IsActive = p.IsActive,
                        CreatedOn = p.CreatedOn
                    }).ToList()
                }).ToList();

                return AppResult<List<ModuleDto>>.Success(moduleDtos);
            }
            catch (Exception ex)
            {
                return AppResult<List<ModuleDto>>.Failure($"An error occurred while retrieving modules: {ex.Message}");
            }
        }
    }
}