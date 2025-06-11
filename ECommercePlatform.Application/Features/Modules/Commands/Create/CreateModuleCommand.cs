using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Modules.Commands.Create
{
    public class CreateModuleCommand : IRequest<AppResult<ModuleDto>>, ITransactionalBehavior, IAuditableCreateRequest
    {
        public required string Name { get; init; }
        public required string Route { get; init; }
        public string? Description { get; init; }
        public string? Icon { get; init; }
        public int DisplayOrder { get; init; }
        public bool IsActive { get; init; } = true;

        [JsonIgnore]
        public string? CreatedBy { get; set; }

        [JsonIgnore]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}