using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Commands.Delete
{
    public record DeleteModuleCommand(Guid Id) : IRequest<AppResult>, ITransactionalBehavior;
}