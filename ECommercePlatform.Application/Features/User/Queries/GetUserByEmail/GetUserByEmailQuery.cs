using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Queries.GetUserByEmail
{
    public record GetUserByEmailQuery(string Email) : IRequest<AppResult<UserDto>>;
}