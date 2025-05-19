using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Queries.GetAllCountries;

public record GetAllCountriesQuery : IRequest<AppResult<List<CountryDto>>>;
