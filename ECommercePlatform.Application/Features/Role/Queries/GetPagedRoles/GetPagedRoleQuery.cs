using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Queries.GetPagedRoles
{
    public class GetPagedRolesQuery : PagedRequest, IRequest<AppResult<PagedResponse<RoleDto>>>
    {
        public bool ActiveOnly { get; set; } = true;
    }
}