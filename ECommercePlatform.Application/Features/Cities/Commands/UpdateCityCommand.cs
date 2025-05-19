using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands
{
    public record UpdateCityCommand : IRequest<AppResult>
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public string? ModifiedBy { get; init; }
        public bool IsActive { get; init; } = true;
        public Guid StateId { get; init; }
    }
}
