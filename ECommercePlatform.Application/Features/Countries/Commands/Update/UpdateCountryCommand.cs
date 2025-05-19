using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Commands.Update;

public record UpdateCountryCommand : IRequest<AppResult>
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Code { get; init; }
    public string? ModifiedBy { get; init; }
    public bool IsActive { get; init; } = true;
}
