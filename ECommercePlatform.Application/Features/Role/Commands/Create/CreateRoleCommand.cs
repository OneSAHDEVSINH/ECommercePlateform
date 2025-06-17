using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Role.Commands.Create
{
    public class CreateRoleCommand : IRequest<AppResult<RoleDto>>, ITransactionalBehavior, IAuditableCreateRequest
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public bool IsActive { get; init; } = true;
        public List<CreateRolePermissionDto> Permissions { get; init; } = new();

        [JsonIgnore]
        public string? CreatedBy { get; set; }

        [JsonIgnore]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public class CreateRolePermissionDto
    {
        public required Guid ModuleId { get; init; }
        public required string PermissionType { get; init; } // Use string to match API
    }
}