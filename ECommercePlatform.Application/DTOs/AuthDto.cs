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
    }

    public class UserDto
    {
        public string? Id { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? Email { get; init; }
        public string? Role { get; init; }
        public bool IsActive { get; init; }
    }
}