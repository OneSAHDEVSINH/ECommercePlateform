using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Queries.GetRoleWithPermissions
{
    public record GetRoleWithPermissionsQuery(Guid Id) : IRequest<AppResult<RoleDto>>;
}