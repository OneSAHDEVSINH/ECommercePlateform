using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Create
{
    public record CreateCityCommand : IRequest<AppResult<CityDto>>, ITransactionalBehavior, IAuditableCreateRequest
    {
        public required string Name { get; init; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public Guid StateId { get; init; }
    }
}
