using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Role.Queries.GetRoleById
{
    public class GetRoleByIdQuery : IRequest<RoleDto>
    {
        public Guid RoleId { get; set; }

        public GetRoleByIdQuery(Guid roleId)
        {
            RoleId = roleId;
        }
    }
}