using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Users.Commands.UploadAvatar
{
    public record UploadAvatarCommand : IRequest<AppResult<string>>, ITransactionalBehavior
    {
        public Guid UserId { get; set; }

        public required IFormFile File { get; init; }
    }
}
