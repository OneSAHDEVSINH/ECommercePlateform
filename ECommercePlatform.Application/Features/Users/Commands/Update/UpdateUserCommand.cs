using ECommercePlatform.Application.Common.Interfaces;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace ECommercePlatform.Application.Features.Users.Commands.Update
{
    public record UpdateUserCommand : IRequest<AppResult<UserDto>>, ITransactionalBehavior, IAuditableUpdateRequest
    {
        public required Guid Id { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? Email { get; init; }
        public string? Password { get; init; } // Optional for updates
        public string? PhoneNumber { get; init; }
        //public Gender? Gender { get; init; }
        //public DateOnly? DateOfBirth { get; init; }
        public string? Gender { get; init; }
        public string? DateOfBirth { get; init; }
        public string? Bio { get; init; }
        public List<Guid>? RoleIds { get; init; }
        public bool? IsActive { get; init; }

        [JsonIgnore]
        public string? ModifiedBy { get; set; }

        [JsonIgnore]
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
    }
}