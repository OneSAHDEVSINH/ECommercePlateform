using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.Cities.Commands
{
    public record CreateCityCommand : IRequest<AppResult<CityDto>>
    {
        public required string Name { get; init; }
        public Guid StateId { get; init; }
    }
}
