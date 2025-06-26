using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Roles.Commands.Create
{
    public class CreateRoleCommand : IRequest<AppResult<RoleDto>>, ITransactionalBehavior, IAuditableCreateRequest
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public bool IsActive { get; init; } = true;
        public List<ModulePermissionDto>? Permissions { get; init; }

        [JsonIgnore]
        public string? CreatedBy { get; set; }

        [JsonIgnore]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public class ModulePermissionDto
    {
        public required Guid ModuleId { get; init; }
        public bool CanView { get; init; }
        public bool CanAddEdit { get; init; }
        public bool CanDelete { get; init; }
    }
}