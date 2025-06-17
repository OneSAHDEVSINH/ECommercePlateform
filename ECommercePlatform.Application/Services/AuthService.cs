using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using ECommercePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommercePlatform.Application.Services
{
    public class AuthService(IConfiguration configuration, IUserRepository userRepository, IUnitOfWork unitOfWork) : IAuthService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                throw new ArgumentException("Email and password are required");

            var user = await _userRepository.FindUserByEmailAndPasswordAsync(loginDto.Email, loginDto.Password)
                ?? throw new KeyNotFoundException("Invalid email or password");

            // Fetch user with roles
            var userWithRoles = await _userRepository.FindUserWithRolesByEmailAsync(loginDto.Email);
            var token = GenerateJwtToken(userWithRoles);

            // Map roles to RoleDto objects
            var roleDtos = userWithRoles.UserRoles?
                .Where(ur => ur.Role != null)
                .Select(ur => new RoleDto
                {
                    Id = ur.Role.Id,
                    Name = ur.Role.Name,
                    Description = ur.Role.Description,
                    IsActive = ur.Role.IsActive
                })
                .ToList() ?? new List<RoleDto>();

            // Get user permissions
            var permissions = await GetUserPermissionsAsync(userWithRoles.Id);

            return new AuthResultDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = userWithRoles.Id,
                    FirstName = userWithRoles.FirstName,
                    LastName = userWithRoles.LastName,
                    Email = userWithRoles.Email,
                    Password = userWithRoles.PasswordHash, // Use PasswordHash
                    Roles = roleDtos,
                    IsActive = userWithRoles.IsActive,
                    PhoneNumber = userWithRoles.PhoneNumber,
                    Gender = userWithRoles.Gender,
                    DateOfBirth = userWithRoles.DateOfBirth,
                    Bio = userWithRoles.Bio,
                    CreatedOn = userWithRoles.CreatedOn
                },
                Permissions = permissions // ADD THIS
            };
        }

        public async Task<List<UserPermissionDto>> GetUserPermissionsAsync(Guid userId)
        {
            // Get user roles
            var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(userId);
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

            if (!roleIds.Any())
                return new List<UserPermissionDto>();

            // Get all role permissions for these roles
            var rolePermissions = await _unitOfWork.RolePermissions.AsQueryable()
                .Include(rp => rp.Module)
                .Where(rp => roleIds.Contains(rp.RoleId) &&
                            rp.Module.IsActive &&
                            !rp.Module.IsDeleted &&
                            rp.IsActive &&
                            !rp.IsDeleted)
                .ToListAsync();

            // Group by module and aggregate permissions
            var permissions = rolePermissions
                .GroupBy(rp => new { rp.ModuleId, rp.Module.Name })
                .Select(g => new UserPermissionDto
                {
                    ModuleName = g.Key.Name,
                    CanView = g.Any(rp => rp.CanView),
                    CanAdd = g.Any(rp => rp.CanAdd),
                    CanEdit = g.Any(rp => rp.CanEdit),
                    CanDelete = g.Any(rp => rp.CanDelete)
                })
                .ToList();

            return permissions;
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
            if (user.Email?.ToLower() == "admin@admin.com")
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