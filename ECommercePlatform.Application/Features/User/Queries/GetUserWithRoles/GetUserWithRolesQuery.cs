using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Queries.GetUserWithRoles
{
    public record GetUserWithRolesQuery(Guid Id) : IRequest<AppResult<UserDto>>;
}