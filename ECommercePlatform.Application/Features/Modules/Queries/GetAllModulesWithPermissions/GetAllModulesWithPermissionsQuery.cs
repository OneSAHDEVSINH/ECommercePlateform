using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetAllModulesWithPermissions
{
    public class GetAllModulesWithPermissionsQuery : IRequest<AppResult<List<ModuleDto>>>
    {
        public bool ActiveOnly { get; set; } = true;
    }
}