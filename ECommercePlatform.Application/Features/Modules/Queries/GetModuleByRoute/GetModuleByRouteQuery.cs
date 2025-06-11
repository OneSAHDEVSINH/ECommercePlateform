using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetModuleByRoute
{
    public record GetModuleByRouteQuery(string Route) : IRequest<AppResult<ModuleDto>>;
}