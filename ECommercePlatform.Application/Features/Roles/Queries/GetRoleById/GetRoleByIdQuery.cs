using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Roles.Queries.GetRoleById
{
    public record GetRoleByIdQuery(Guid Id) : IRequest<AppResult<RoleDto>>;
}