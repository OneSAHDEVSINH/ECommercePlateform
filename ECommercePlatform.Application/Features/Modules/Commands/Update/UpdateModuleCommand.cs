using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Modules.Commands.Update
{
    public class UpdateModuleCommand(string Name, string Route, string Description, string Icon, int DisplayOrder) : IRequest<AppResult>, ITransactionalBehavior, IAuditableUpdateRequest
    {
        public required Guid Id { get; init; }
        public string? Name { get; init; } = Name?.Trim() ?? string.Empty;
        public string? Route { get; init; } = Route?.Trim() ?? string.Empty;
        public string? Description { get; init; } = Description?.Trim() ?? string.Empty;
        public string? Icon { get; init; } = Icon?.Trim() ?? string.Empty;
        public int? DisplayOrder { get; init; } = DisplayOrder > 0 ? DisplayOrder : null;
        public bool IsActive { get; init; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
    }
}