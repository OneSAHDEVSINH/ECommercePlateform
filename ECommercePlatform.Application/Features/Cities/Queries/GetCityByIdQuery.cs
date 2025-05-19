using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Queries
{
    public record GetCityByIdQuery(Guid Id) : IRequest<AppResult<CityDto>>;

}
