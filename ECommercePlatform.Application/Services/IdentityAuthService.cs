using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IServices;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using ECommercePlatform.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommercePlatform.Application.Services
{
    public class IdentityAuthService(
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IPermissionService permissionService,
        ISuperAdminService superAdminService) : IAuthService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPermissionService _permissionService = permissionService;
        private readonly ISuperAdminService _superAdminService = superAdminService;

        public async Task<AppResult<AuthResultDto>> LoginAsync(LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                return AppResult<AuthResultDto>.Failure("Email and password are required");

            // Find user by email
            var user = await _unitOfWork.UserManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return AppResult<AuthResultDto>.Failure("Invalid email or password");

            // Check if user account is active
            if (!user.IsActive || user.IsDeleted)
                return AppResult<AuthResultDto>.Failure("User account is inactive or deleted");

            // Check if user is superadmin - this should bypass role requirements
            bool isSuperAdmin = _superAdminService.IsSuperAdminEmail(user.Email!);

            // Check password
            var result = await _unitOfWork.SignInManager.CheckPasswordSignInAsync(
                user, loginDto.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
                return AppResult<AuthResultDto>.Failure("Invalid email or password");

            // Get user roles
            var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);

            // CHECK: Non-superadmin users must have at least one role
            if (!isSuperAdmin && !userRoles.Any())
            {
                return AppResult<AuthResultDto>.Failure(
                    "Access denied. You don't have any assigned roles. Please contact the administrator.");
            }

            var roles = new List<RoleDto>();
            foreach (var roleName in userRoles)
            {
                var role = await _unitOfWork.RoleManager.FindByNameAsync(roleName);
                if (role != null && role.IsActive && !role.IsDeleted)
                {
                    roles.Add(new RoleDto
                    {
                        Id = role.Id,
                        Name = role.Name,
                        Description = role.Description,
                        IsActive = role.IsActive
                    });
                }
            }

            // Check if non-superadmin user has any active roles
            if (!isSuperAdmin && roles.Count == 0)
            {
                return AppResult<AuthResultDto>.Failure(
                    "Access denied. All your assigned roles are inactive. Please contact the administrator.");
            }

            // Get user permissions
            var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);

            // Generate JWT token
            var token = await GenerateJwtTokenAsync(user, userRoles);

            return AppResult<AuthResultDto>.Success(new AuthResultDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    Bio = user.Bio,
                    Roles = roles,
                    IsActive = user.IsActive,
                    CreatedOn = user.CreatedOn
                },
                Permissions = permissions
            });
        }

        public async Task<List<UserPermissionDto>> GetUserPermissionsAsync(Guid userId)
        {
            return await _permissionService.GetUserPermissionsAsync(userId);
        }

        private async Task<string> GenerateJwtTokenAsync(User user, IList<string> roles)
        {
            if (user?.Email == null)
                throw new ArgumentNullException(nameof(user), "User or email cannot be null");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            // Add roles as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add SuperAdmin claim if user is the superadmin
            if (_superAdminService.IsSuperAdminEmail(user.Email))
            {
                claims.Add(new Claim("SuperAdmin", "true"));
            }

            // Don't add individual permissions to JWT to keep token size small
            // Permissions will be checked from database on each request

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(24),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}