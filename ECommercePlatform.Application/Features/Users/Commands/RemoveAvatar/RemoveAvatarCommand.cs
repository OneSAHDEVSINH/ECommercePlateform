using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Users.Commands.RemoveAvatar
{
    public class RemoveAvatarCommand : IRequest<AppResult>, ITransactionalBehavior
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}