using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Roles.Commands.Delete
{
    public record DeleteRoleCommand(Guid Id) : IRequest<AppResult>, ITransactionalBehavior;
}