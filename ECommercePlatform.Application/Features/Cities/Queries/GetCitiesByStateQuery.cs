using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.Cities.Queries
{
    public record GetCitiesByStateQuery(Guid StateId) : IRequest<AppResult<List<CityDto>>>;

}
