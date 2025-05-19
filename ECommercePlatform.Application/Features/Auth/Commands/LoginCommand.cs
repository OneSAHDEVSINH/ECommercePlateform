using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.Auth.Commands
{
    public record LoginCommand : IRequest<AppResult<AuthResultDto>>
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
