using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Role.Commands.Update
{
    public class UpdateRoleCommand : IRequest<AppResult<RoleDto>>, ITransactionalBehavior, IAuditableUpdateRequest
    {
        public required Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public bool? IsActive { get; init; }
        public List<UpdateRolePermissionDto>? Permissions { get; init; }

        [JsonIgnore]
        public string? ModifiedBy { get; set; }

        [JsonIgnore]
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
    }

    public class UpdateRolePermissionDto
    {
        public required Guid ModuleId { get; init; }
        public required string PermissionType { get; init; }
    }
}