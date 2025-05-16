using System;

namespace ECommercePlatform.Server.Core.Application.DTOs
{
    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class AuthResultDto
    {
        public string? Token { get; set; }
        public UserDto? User { get; set; }
    }

    public class UserDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
    }
} 