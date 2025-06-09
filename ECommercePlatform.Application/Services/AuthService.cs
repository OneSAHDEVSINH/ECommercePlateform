using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using ECommercePlatform.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommercePlatform.Application.Services
{
    public class AuthService(IConfiguration configuration, IUserRepository userRepository) : IAuthService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                throw new ArgumentException("Email and password are required");

            var user = await _userRepository.FindUserByEmailAndPasswordAsync(loginDto.Email, loginDto.Password)
                ?? throw new KeyNotFoundException("Invalid email or password");

            // Fetch user with roles
            var userWithRoles = await _userRepository.FindUserWithRolesByEmailAsync(loginDto.Email);
            var token = GenerateJwtToken(userWithRoles);

            // Map roles to RoleDto objects, not just strings
            var roleDtos = userWithRoles.UserRoles?
                .Where(ur => ur.Role != null)
                .Select(ur => new RoleDto
                {
                    Id = ur.Role.Id,
                    Name = ur.Role.Name,
                    Description = ur.Role.Description,
                    IsActive = ur.Role.IsActive
                    // Permissions would be null here since we're not loading them
                })
                .ToList() ?? new List<RoleDto>();

            return new AuthResultDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = userWithRoles.Id,
                    FirstName = userWithRoles.FirstName,
                    LastName = userWithRoles.LastName,
                    Email = userWithRoles.Email,
                    Roles = roleDtos, // Use the properly mapped RoleDto objects
                    IsActive = userWithRoles.IsActive
                }
            };
        }

        //private string GenerateJwtToken(User user)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException(nameof(user), "User cannot be null");
        //    if (user.Email == null)
        //        throw new ArgumentNullException(nameof(user), "User email cannot be null");

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "MyTemporarySecretKeyForDevelopment12345");

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(
        //        [
        //            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //            new Claim(ClaimTypes.Email, user.Email),
        //            //new Claim(ClaimTypes.Role, user.Role.ToString())
        //        ]),
        //        Expires = DateTime.Now.AddHours(24),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        //        Issuer = _configuration["Jwt:Issuer"],
        //        Audience = _configuration["Jwt:Audience"]
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}

        private string GenerateJwtToken(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            if (user.Email == null)
                throw new ArgumentNullException(nameof(user), "User email cannot be null");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "MyTemporarySecretKeyForDevelopment12345");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Add all roles as claims
            if (user.UserRoles != null)
            {
                foreach (var userRole in user.UserRoles)
                {
                    if (userRole.Role != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                    }
                }
            }

            // Optionally: Add SuperAdmin claim if this is the seeded admin
            if (user.Email == "admin@admin.com")
            {
                claims.Add(new Claim("SuperAdmin", "true"));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}