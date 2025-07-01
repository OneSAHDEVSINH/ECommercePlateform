using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Queries.GetAllCities
{
    public record GetAllCitiesQuery(bool ActiveOnly = true) : IRequest<AppResult<List<CityDto>>>;
}
