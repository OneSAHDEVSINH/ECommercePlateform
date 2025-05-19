using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Auth.Queries
{
    public record GetCurrentUserQuery : IRequest<AppResult<UserDto>>;
}
