using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Queries.GetStatesByCountry
{
    public record GetStatesByCountryQuery(Guid CountryId) : IRequest<AppResult<List<StateDto>>>;
}
