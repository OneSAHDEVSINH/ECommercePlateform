using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Users.Queries.GetAllUsers
{
    public record GetAllUsersQuery(bool ActiveOnly = true) : IRequest<AppResult<List<UserDto>>>;
}