using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.Countries.Queries.GetPagedCountries
{
    public class GetPagedCountriesQuery : PagedRequest, IRequest<AppResult<PagedResponse<CountryDto>>>
    {
    }
}
