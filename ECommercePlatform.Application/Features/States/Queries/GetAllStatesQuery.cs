using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Queries
{
    public record GetAllStatesQuery : IRequest<AppResult<List<StateDto>>>;
}
