using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Modules.Commands.Update
{
    public class UpdateModuleCommand : IRequest<AppResult<ModuleDto>>, ITransactionalBehavior, IAuditableUpdateRequest
    {
        public required Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Route { get; init; }
        public string? Description { get; init; }
        public string? Icon { get; init; }
        public int? DisplayOrder { get; init; }
        public bool? IsActive { get; init; }

        [JsonIgnore]
        public string? ModifiedBy { get; set; }

        [JsonIgnore]
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
    }
}