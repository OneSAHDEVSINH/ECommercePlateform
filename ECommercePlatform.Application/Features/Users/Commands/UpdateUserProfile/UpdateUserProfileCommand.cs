// Application/Features/Users/Commands/UpdateUserProfile/UpdateUserProfileCommand.cs
using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Domain.Enums;
using MediatR;

namespace ECommercePlatform.Application.Features.Users.Commands.UpdateUserProfile
{
    public record UpdateUserProfileCommand : IRequest<AppResult<UserProfileDto>>, ITransactionalBehavior
    {
        public Guid Id { get; set; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? PhoneNumber { get; init; }
        public Gender? Gender { get; init; }
        public DateOnly? DateOfBirth { get; init; }
        public string? Bio { get; init; }
    }
}