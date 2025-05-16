using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.Countries.Commands;

public record CreateCountryCommand : IRequest<AppResult<CountryDto>>
{
    public required string Name { get; init; }
    public required string Code { get; init; }
}
