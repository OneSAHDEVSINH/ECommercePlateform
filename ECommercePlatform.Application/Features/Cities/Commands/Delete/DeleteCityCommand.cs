using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Delete
{
    public record DeleteCityCommand(Guid Id) : IRequest<AppResult>, ITransactionalBehavior;
}
