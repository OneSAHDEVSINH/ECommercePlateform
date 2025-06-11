using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Queries.GetRolesByPermissionId
{
    public record GetRolesByPermissionIdQuery(Guid PermissionId, bool ActiveOnly = true) : IRequest<AppResult<List<RoleDto>>>;
}