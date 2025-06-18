// Application/Features/Users/Commands/ChangePassword/ChangePasswordCommand.cs
using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest<AppResult>, ITransactionalBehavior
    {
        [JsonIgnore]
        public Guid UserId { get; set; } // Set by controller

        public required string CurrentPassword { get; init; }
        public required string NewPassword { get; init; }
    }
}