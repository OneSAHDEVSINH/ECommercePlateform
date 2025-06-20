using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Users.Commands.Create
{
    public record CreateUserCommand : IRequest<AppResult<UserDto>>, ITransactionalBehavior, IAuditableCreateRequest
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Email { get; init; }
        public required string Password { get; init; }
        public string? PhoneNumber { get; init; }
        //public Gender Gender { get; init; }
        public string? Gender { get; init; }
        //public DateOnly? DateOfBirth { get; init; }
        public string? DateOfBirth { get; init; }
        public string? Bio { get; init; }
        public List<Guid>? RoleIds { get; init; } = new();
        public bool IsActive { get; init; } = true;

        [JsonIgnore]
        public string? CreatedBy { get; set; }

        [JsonIgnore]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}