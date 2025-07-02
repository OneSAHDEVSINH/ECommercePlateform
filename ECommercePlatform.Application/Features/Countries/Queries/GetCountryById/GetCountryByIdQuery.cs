using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Queries.GetCountryById
{
    public record GetCountryByIdQuery(Guid Id) : IRequest<AppResult<CountryDto>>;
}