using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetModuleWithPermissions
{
    public record GetModuleWithPermissionsQuery(Guid Id) : IRequest<AppResult<ModuleDto>>;
}