using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Commands.Delete;

public record DeleteStateCommand(Guid Id) : IRequest<AppResult>, ITransactionalBehavior;
