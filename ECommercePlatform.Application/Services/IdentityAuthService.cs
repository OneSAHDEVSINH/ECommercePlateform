using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using ECommercePlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommercePlatform.Application.Services
{
    public class IdentityAuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public IdentityAuthService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                throw new ArgumentException("Email and password are required");

            // Find user by email
            var user = await _unitOfWork.UserManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                throw new KeyNotFoundException("Invalid email or password");

            // Check if user account is active
            if (!user.IsActive || user.IsDeleted)
                throw new KeyNotFoundException("User account is inactive or deleted");

            // Check password
            var result = await _unitOfWork.SignInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
                throw new KeyNotFoundException("Invalid email or password");

            // Get user roles
            var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);
            var roles = new List<RoleDto>();

            foreach (var roleName in userRoles)
            {
                var role = await _unitOfWork.RoleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    roles.Add(new RoleDto
                    {
                        Id = Guid.Parse(role.Id),
                        Name = role.Name,
                        Description = role.Description,
                        IsActive = role.IsActive
                    });
                }
            }

            // Generate JWT token
            var token = await GenerateJwtTokenAsync(user, userRoles);

            return new AuthResultDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = user.Password,
                    Roles = roles,
                    IsActive = user.IsActive
                }
            };
        }

        private async Task<string> GenerateJwtTokenAsync(User user, IList<string> roles)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            if (user.Email == null)
                throw new ArgumentNullException(nameof(user), "User email cannot be null");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "MyTemporarySecretKeyForDevelopment12345");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Add all roles as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                
                // Check if this is the SuperAdmin role
                if (role == "SuperAdmin")
                {
                    claims.Add(new Claim("SuperAdmin", "true"));
                }
            }

            // Add user permissions based on roles
            var userPermissions = new HashSet<string>();
            
            // Get all modules with their permissions
            var modules = await _unitOfWork.Modules.GetAllModulesWithPermissionsAsync();
            
            foreach (var role in roles)
            {
                // Get role by name
                var appRole = await _unitOfWork.RoleManager.FindByNameAsync(role);
                if (appRole != null)
                {
                    // Get role permissions
                    var rolePermissions = await _unitOfWork.RolePermissions.GetByRoleIdAsync(Guid.Parse(appRole.Id));
                    
                    foreach (var permission in rolePermissions)
                    {
                        // Find the module and permission type
                        var module = modules.FirstOrDefault(m => m.Permissions.Any(p => p.Id == permission.PermissionId));
                        if (module != null)
                        {
                            var modulePermission = module.Permissions.FirstOrDefault(p => p.Id == permission.PermissionId);
                            if (modulePermission != null)
                            {
                                var permissionKey = $"{module.Route}:{modulePermission.Type}";
                                userPermissions.Add(permissionKey);
                            }
                        }
                    }
                }
            }

            // Add module permissions as claims
            foreach (var permission in userPermissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
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