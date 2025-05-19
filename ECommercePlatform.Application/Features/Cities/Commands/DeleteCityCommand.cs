using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands
{
    public record DeleteCityCommand(Guid Id) : IRequest<AppResult>;
}
