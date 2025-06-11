using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using MediatR;

namespace ECommercePlatform.Application.Features.User.Queries.GetPagedUsers
{
    public class GetPagedUsersQuery : PagedRequest, IRequest<AppResult<PagedResponse<UserDto>>>
    {
        public bool ActiveOnly { get; set; } = true;
    }
}