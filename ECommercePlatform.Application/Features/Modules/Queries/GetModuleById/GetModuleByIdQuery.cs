using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetModuleById
{
    public record GetModuleByIdQuery(Guid Id) : IRequest<AppResult<ModuleDto>>;
}