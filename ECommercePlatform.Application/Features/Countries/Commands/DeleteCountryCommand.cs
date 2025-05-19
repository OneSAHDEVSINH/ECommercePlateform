using ECommercePlatform.Application.Common.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.Countries.Commands;

public record DeleteCountryCommand(Guid Id) : IRequest<AppResult>;
