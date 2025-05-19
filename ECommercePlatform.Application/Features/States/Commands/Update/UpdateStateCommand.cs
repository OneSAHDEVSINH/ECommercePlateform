using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Commands.Update
{
    public record UpdateStateCommand : IRequest<AppResult>, ITransactionalBehavior
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Code { get; init; }
        public string? ModifiedBy { get; init; }
        public Guid CountryId { get; init; }
        public bool IsActive { get; init; } = true;
    }
}
