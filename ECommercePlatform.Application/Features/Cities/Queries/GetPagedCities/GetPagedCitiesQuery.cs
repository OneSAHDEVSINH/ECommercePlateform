using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Queries.GetPagedCities
{
    public class GetPagedCitiesQuery : PagedRequest, IRequest<AppResult<PagedResponse<CityDto>>>
    {
        public Guid? StateId { get; set; }
        public Guid? CountryId { get; set; }
    }
}
