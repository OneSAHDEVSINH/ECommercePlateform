using ECommercePlatform.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.States.Commands
{
    public record UpdateStateCommand : IRequest<AppResult>
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Code { get; init; }
        public Guid CountryId { get; init; }
        public bool IsActive { get; init; } = true;
    }
}
