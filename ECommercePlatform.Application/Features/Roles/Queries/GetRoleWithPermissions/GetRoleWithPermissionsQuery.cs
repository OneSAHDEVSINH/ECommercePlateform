using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Roles.Queries.GetRoleWithPermissions
{
    public record GetRoleWithPermissionsQuery(Guid Id) : IRequest<AppResult<RoleDto>>;
}