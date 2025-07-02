using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Queries.GetAllCountries
{
    public record GetAllCountriesQuery(bool ActiveOnly = true) : IRequest<AppResult<List<CountryDto>>>;
}

