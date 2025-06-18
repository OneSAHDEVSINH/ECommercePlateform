using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Features.Roles.Commands.Create;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Roles.Commands.Update
{
    public class UpdateRoleCommand : IRequest<AppResult<RoleDto>>, ITransactionalBehavior, IAuditableUpdateRequest
    {
        public required Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public bool? IsActive { get; init; }
        public List<ModulePermissionDto>? Permissions { get; init; } // Use ModulePermissionDto, not UpdateRolePermissionDto

        [JsonIgnore]
        public string? ModifiedBy { get; set; }

        [JsonIgnore]
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
    }
}