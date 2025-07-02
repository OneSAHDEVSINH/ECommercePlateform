using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Users.Commands.RemoveAvatar
{
    public record RemoveAvatarCommand : IRequest<AppResult>, ITransactionalBehavior
    {
        public Guid UserId { get; set; }
    }
}