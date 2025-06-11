using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Commands.Delete
{
    public record DeleteUserCommand(Guid Id) : IRequest<AppResult>, ITransactionalBehavior;
}