using ECommercePlatform.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.Cities.Commands
{
    public record  UpdateCityCommand : IRequest<AppResult>
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public bool IsActive { get; init; } = true;
        public Guid StateId { get; init; }
    }
}
