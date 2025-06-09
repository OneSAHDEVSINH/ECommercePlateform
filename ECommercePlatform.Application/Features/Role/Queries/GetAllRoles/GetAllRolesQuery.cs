using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Queries.GetAllRoles
{
    public class GetAllRolesQuery : IRequest<List<RoleDto>>
    {
    }
}