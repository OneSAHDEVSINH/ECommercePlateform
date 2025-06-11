using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Queries.GetUsersByRoleId
{
    public record GetUsersByRoleIdQuery(Guid RoleId, bool ActiveOnly = true) : IRequest<AppResult<List<UserDto>>>;
}