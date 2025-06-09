using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules
{
    public class GetAllModulesWithPermissionsHandler : IRequestHandler<GetAllModulesWithPermissionsQuery, List<ModuleDto>>
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public GetAllModulesWithPermissionsHandler(IModuleRepository moduleRepository, IPermissionRepository permissionRepository)
        {
            _moduleRepository = moduleRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<List<ModuleDto>> Handle(GetAllModulesWithPermissionsQuery request, CancellationToken cancellationToken)
        {
            var modules = await _moduleRepository.GetAllAsync();
            var allPermissions = await _permissionRepository.GetAllAsync();
            var result = new List<ModuleDto>();

            foreach (var module in modules)
            {
                var permissions = allPermissions.Where(p => p.ModuleId == module.Id).Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Type = p.Type.ToString(),
                    ModuleId = p.ModuleId,
                    ModuleName = module.Name
                }).ToList();

                result.Add(new ModuleDto
                {
                    Id = module.Id,
                    Name = module.Name,
                    Description = module.Description,
                    Route = module.Route,
                    Icon = module.Icon,
                    DisplayOrder = module.DisplayOrder,
                    Permissions = permissions
                });
            }
            return result;
        }
    }
}