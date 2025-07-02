using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Roles.Commands.Create
{
    public record CreateRoleCommand(string Name, string Description) : IRequest<AppResult<RoleDto>>, ITransactionalBehavior, IAuditableCreateRequest
    {
        public required string Name { get; init; } = Name?.Trim() ?? string.Empty;
        public string? Description { get; init; } = Description?.Trim() ?? string.Empty;
        public bool IsActive { get; init; } = true;
        public List<ModulePermissionDto>? Permissions { get; init; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}