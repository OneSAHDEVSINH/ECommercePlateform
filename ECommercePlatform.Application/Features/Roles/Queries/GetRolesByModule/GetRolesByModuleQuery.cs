// GetRolesByModuleQuery.cs
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Roles.Queries.GetRolesByModule
{
    public record GetRolesByModuleQuery(Guid ModuleId, bool ActiveOnly = true) : IRequest<AppResult<List<RoleDto>>>;
}