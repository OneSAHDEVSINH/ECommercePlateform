using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Queries.GetPagedCountries
{
    public class GetPagedCountriesQuery : PagedRequest, IRequest<AppResult<PagedResponse<CountryDto>>>
    {
    }
}
