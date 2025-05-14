using ECommercePlateform.Application.DTOs;
using ECommercePlateform.Application.Interfaces.IAuth;
using ECommercePlateform.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommercePlateform.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                throw new ArgumentException("Email and password are required");

            var user = await _userRepository.FindUserByEmailAndPasswordAsync(loginDto.Email, loginDto.Password);
            if (user == null)
                throw new KeyNotFoundException("Invalid email or password");

            var token = GenerateJwtToken(user);

            return new AuthResultDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id.ToString(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive
                }
            };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "MyTemporarySecretKeyForDevelopment12345");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}