// Application/Features/Users/Commands/ChangePassword/ChangePasswordCommand.cs
using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Users.Commands.ChangePassword
{
    public record ChangePasswordCommand : IRequest<AppResult>, ITransactionalBehavior
    {
        public Guid UserId { get; set; }
        public required string CurrentPassword { get; init; }
        public required string NewPassword { get; init; }
    }
}