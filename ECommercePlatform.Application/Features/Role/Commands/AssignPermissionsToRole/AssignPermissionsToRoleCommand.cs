using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Role.Commands.AssignPermissionsToRole
{
    public class AssignPermissionsToRoleCommand : IRequest<AppResult>, ITransactionalBehavior, IAuditableUpdateRequest
    {
        public required Guid RoleId { get; init; }
        public required List<RolePermissionAssignmentDto> Permissions { get; init; } = new();

        [JsonIgnore]
        public string? ModifiedBy { get; set; }

        [JsonIgnore]
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
    }

    public class RolePermissionAssignmentDto
    {
        public required Guid ModuleId { get; init; }
        public required List<string> PermissionTypes { get; init; } = new();
    }
}