using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Users.Commands.AssignRolesToUser
{
    public record AssignRolesToUserCommand : IRequest<AppResult>, ITransactionalBehavior, IAuditableUpdateRequest
    {
        public required Guid UserId { get; init; }
        public required List<Guid> RoleIds { get; init; } = [];

        [JsonIgnore]
        public string? ModifiedBy { get; set; }

        [JsonIgnore]
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
    }
}