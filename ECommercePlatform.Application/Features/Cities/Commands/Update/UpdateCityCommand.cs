using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Update
{
    public record UpdateCityCommand : IRequest<AppResult>, ITransactionalBehavior, IAuditableUpdateRequest
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public bool IsActive { get; init; } = true;
        public Guid StateId { get; init; }
    }
}
