using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Queries.GetUserById
{
    public record GetUserByIdQuery(Guid Id) : IRequest<AppResult<UserDto>>;
}