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
}