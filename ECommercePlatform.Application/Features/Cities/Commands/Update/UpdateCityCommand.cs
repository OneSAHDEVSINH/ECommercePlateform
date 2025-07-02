using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Update
{
    public record UpdateCityCommand(string Name) : IRequest<AppResult>, ITransactionalBehavior, IAuditableUpdateRequest
    {
        public Guid Id { get; init; }
        public required string Name { get; init; } = Name?.Trim() ?? string.Empty;
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public bool IsActive { get; init; }
        public Guid StateId { get; init; }
    }
}
