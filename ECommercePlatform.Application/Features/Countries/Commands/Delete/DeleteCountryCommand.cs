using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Commands.Delete;

public record DeleteCountryCommand(Guid Id) : IRequest<AppResult>;
