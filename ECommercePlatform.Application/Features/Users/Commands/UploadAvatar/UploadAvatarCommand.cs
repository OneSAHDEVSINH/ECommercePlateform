using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Users.Commands.UploadAvatar
{
    public class UploadAvatarCommand : IRequest<AppResult<string>>, ITransactionalBehavior
    {
        [JsonIgnore]
        public Guid UserId { get; set; }

        public required IFormFile File { get; init; }
    }
}
