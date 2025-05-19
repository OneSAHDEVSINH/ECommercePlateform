using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Queries.GetCitiesByState
{
    public record GetCitiesByStateQuery(Guid StateId) : IRequest<AppResult<List<CityDto>>>;

}
