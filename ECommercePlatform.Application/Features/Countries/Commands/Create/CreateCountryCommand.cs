using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Commands.Create;

public record CreateCountryCommand : IRequest<AppResult<CountryDto>>, ITransactionalBehavior, IAuditableCreateRequest
{
    public required string Name { get; init; }
    public required string Code { get; init; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public CreateCountryCommand(string name, string code)
    {
        Name = name?.Trim() ?? string.Empty;
        Code = code?.Trim() ?? string.Empty;
    }
}
