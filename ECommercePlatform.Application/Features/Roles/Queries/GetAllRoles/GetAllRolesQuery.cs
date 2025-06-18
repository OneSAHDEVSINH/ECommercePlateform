using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Roles.Queries.GetAllRoles
{
    public record GetAllRolesQuery(bool ActiveOnly = true) : IRequest<AppResult<List<RoleDto>>>;
}