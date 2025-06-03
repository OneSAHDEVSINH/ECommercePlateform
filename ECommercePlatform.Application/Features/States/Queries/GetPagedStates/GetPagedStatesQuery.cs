using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.States.Queries.GetPagedStates
{
    public class GetPagedStatesQuery : PagedRequest, IRequest<AppResult<PagedResponse<StateDto>>>
    {
        public Guid? CountryId { get; set; }
    }
}
