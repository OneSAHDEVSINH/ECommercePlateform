using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetModulePermissions
{
    public record GetModulePermissionsQuery(Guid ModuleId) : IRequest<AppResult<List<RolePermissionDto>>>;
}
