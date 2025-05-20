using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Commands.Update;

public record UpdateCountryCommand : IRequest<AppResult>, ITransactionalBehavior, IAuditableUpdateRequest
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Code { get; init; }
    public string? ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; } = DateTime.Now;
    public bool IsActive { get; init; } = true;
}
