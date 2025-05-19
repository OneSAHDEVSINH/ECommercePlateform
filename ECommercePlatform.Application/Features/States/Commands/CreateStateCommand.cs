using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Commands
{
    public record CreateStateCommand : IRequest<AppResult<StateDto>>
    {
        public required string Name { get; init; }
        public required string Code { get; init; }
        public string? CreatedBy { get; init; }
        public Guid CountryId { get; init; }

    }
}
