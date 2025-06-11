using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Queries.GetAllUsers
{
    public record GetAllUsersQuery(bool ActiveOnly = true) : IRequest<AppResult<List<UserDto>>>;
}