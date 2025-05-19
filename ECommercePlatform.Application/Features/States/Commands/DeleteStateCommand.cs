using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Commands;

public record DeleteStateCommand(Guid Id) : IRequest<AppResult>;
