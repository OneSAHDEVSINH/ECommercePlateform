using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Queries
{
    public record GetStateByIdQuery(Guid Id) : IRequest<AppResult<StateDto>>;
}
