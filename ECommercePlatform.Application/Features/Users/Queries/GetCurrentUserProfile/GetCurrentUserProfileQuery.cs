// Application/Features/Users/Queries/GetCurrentUserProfile/GetCurrentUserProfileQuery.cs
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;

namespace ECommercePlatform.Application.Features.Users.Queries.GetCurrentUserProfile
{
    public record GetCurrentUserProfileQuery(string UserId) : IRequest<AppResult<UserProfileDto>>;
}