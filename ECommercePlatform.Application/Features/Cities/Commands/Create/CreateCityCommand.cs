using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Create
{
    public record CreateCityCommand : IRequest<AppResult<CityDto>>
    {
        public required string Name { get; init; }
        public string? CreatedBy { get; init; }
        public Guid StateId { get; init; }
    }
}
