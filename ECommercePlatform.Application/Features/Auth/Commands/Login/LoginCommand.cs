using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Auth.Commands.Login
{
    public record LoginCommand : IRequest<AppResult<AuthResultDto>>
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
