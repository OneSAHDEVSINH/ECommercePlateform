using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetModuleByRoute
{
    public class GetModuleByRouteHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetModuleByRouteQuery, AppResult<ModuleDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<ModuleDto>> Handle(GetModuleByRouteQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var module = await _unitOfWork.Modules.GetByRouteAsync(request.Route);
                if (module == null)
                    return AppResult<ModuleDto>.Failure($"Module with route '{request.Route}' not found.");

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
                    Permissions = module.Permissions?.Select(p => new PermissionDto
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
                };

                return AppResult<ModuleDto>.Success(moduleDto);
            }
            catch (Exception ex)
            {
                return AppResult<ModuleDto>.Failure($"An error occurred while retrieving the module: {ex.Message}");
            }
        }
    }
}