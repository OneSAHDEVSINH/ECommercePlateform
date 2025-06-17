namespace ECommercePlatform.Application.DTOs
{
    public class LoginDto
    {
        public string? Email { get; init; }
        public string? Password { get; init; }
    }

    public class AuthResultDto
    {
        public string? Token { get; init; }
        public UserDto? User { get; init; }
        public List<UserPermissionDto>? Permissions { get; init; } // ADD THIS
    }

    // ADD THIS NEW DTO
    public class UserPermissionDto
    {
        public required string ModuleName { get; init; }
        public bool CanView { get; init; }
        public bool CanAdd { get; init; }
        public bool CanEdit { get; init; }
        public bool CanDelete { get; init; }
    }
}