using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.States.Commands
{
    public record CreateStateCommand : IRequest<AppResult<StateDto>>
    {
        public required string Name { get; init; }
        public required string Code { get; init; }
        public Guid CountryId { get; init; }
        
    }
}
