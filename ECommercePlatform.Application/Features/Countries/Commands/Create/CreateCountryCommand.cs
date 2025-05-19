using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Commands.Create;

public record CreateCountryCommand : IRequest<AppResult<CountryDto>>, IBaseRequest, IEquatable<CreateCountryCommand>
{
    public required string Name { get; init; }
    public required string Code { get; init; }
    public string? CreatedBy { get; init; }
}
