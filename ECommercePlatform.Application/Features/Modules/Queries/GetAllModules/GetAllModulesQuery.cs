using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetAllModules
{
    public record GetAllModulesQuery(bool ActiveOnly = true) : IRequest<AppResult<List<ModuleDto>>>;
}