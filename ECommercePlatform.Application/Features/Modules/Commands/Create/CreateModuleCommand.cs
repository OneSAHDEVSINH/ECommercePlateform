using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using FluentAssertions.Equivalency;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Modules.Commands.Create
{
    public record CreateModuleCommand(string Name, string Route, string Description, string Icon, int DisplayOrder) : IRequest<AppResult<ModuleDto>>, ITransactionalBehavior, IAuditableCreateRequest
    {
        public required string Name { get; init; } = Name?.Trim() ?? string.Empty;
        public required string Route { get; init; } = Route?.Trim() ?? string.Empty;
        public string? Description { get; init; } = Description?.Trim() ?? string.Empty;
        public string? Icon { get; init; } = Icon?.Trim() ?? string.Empty;
        public int DisplayOrder { get; init; } = DisplayOrder;
        public bool IsActive { get; init; } = true;
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}