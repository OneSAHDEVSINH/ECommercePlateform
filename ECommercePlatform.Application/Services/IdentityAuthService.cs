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
    public class IdentityAuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionService _permissionService;

        public IdentityAuthService(
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IPermissionService permissionService)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _permissionService = permissionService;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                throw new ArgumentException("Email and password are required");

            // Find user by email
            var user = await _unitOfWork.UserManager.FindByEmailAsync(loginDto.Email)
                ?? throw new KeyNotFoundException("Invalid email or password");

            // Check if user account is active
            if (!user.IsActive || user.IsDeleted)
                throw new KeyNotFoundException("User account is inactive or deleted");

            // Check password
            var result = await _unitOfWork.SignInManager.CheckPasswordSignInAsync(
                user, loginDto.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
                throw new KeyNotFoundException("Invalid email or password");

            // Get user roles
            var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);

            // CHECK: User must have at least one role
            if (!userRoles.Any())
            {
                throw new UnauthorizedAccessException(
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

            // Check if user has any active roles
            if (!roles.Any())
            {
                throw new UnauthorizedAccessException(
                    "Access denied. All your assigned roles are inactive. Please contact the administrator.");
            }

            // Get user permissions
            var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);

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
                    
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    Bio = user.Bio,
                    Roles = roles,
                    IsActive = user.IsActive,
                    CreatedOn = user.CreatedOn
                },
                Permissions = permissions
            };
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
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            // Add roles as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

                if (role == "SuperAdmin")
                {
                    claims.Add(new Claim("SuperAdmin", "true"));
                }
            }

            // Don't add individual permissions to JWT to keep token size small
            // Permissions will be checked from database on each request

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
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

//namespace ECommercePlatform.Application.Services
//{
//    public class IdentityAuthService(IConfiguration configuration, IUnitOfWork unitOfWork, IPermissionService permissionService) : IAuthService
//    {
//        private readonly IConfiguration _configuration = configuration;
//        private readonly IUnitOfWork _unitOfWork = unitOfWork;
//        private readonly IPermissionService _permissionService = permissionService;

//        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
//        {
//            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
//                throw new ArgumentException("Email and password are required");

//            // Find user by email
//            var user = await _unitOfWork.UserManager.FindByEmailAsync(loginDto.Email)
//                ?? throw new KeyNotFoundException("Invalid email or password");

//            // Check if user account is active
//            if (!user.IsActive || user.IsDeleted)
//                throw new KeyNotFoundException("User account is inactive or deleted");

//            // Check password
//            var result = await _unitOfWork.SignInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);
//            if (!result.Succeeded)
//                throw new KeyNotFoundException("Invalid email or password");

//            // Get user roles
//            var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);

//            // CHECK: User must have at least one role
//            if (!userRoles.Any())
//            {
//                throw new UnauthorizedAccessException(
//                    "Access denied. You don't have any assigned roles. Please contact the administrator.");
//            }

//            var roles = new List<RoleDto>();

//            foreach (var roleName in userRoles)
//            {
//                var role = await _unitOfWork.RoleManager.FindByNameAsync(roleName);
//                if (role != null)
//                {
//                    roles.Add(new RoleDto
//                    {
//                        Id = role.Id,
//                        Name = role.Name,
//                        Description = role.Description,
//                        IsActive = role.IsActive
//                    });
//                }
//            }

//            // Get user permissions
//            var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);

//            // Generate JWT token with permissions
//            var token = await GenerateJwtTokenAsync(user, userRoles, permissions);

//            return new AuthResultDto
//            {
//                Token = token,
//                User = new UserDto
//                {
//                    Id = user.Id,
//                    FirstName = user.FirstName,
//                    LastName = user.LastName,
//                    Email = user.Email,
//                    PhoneNumber = user.PhoneNumber,
//                    Gender = user.Gender,
//                    DateOfBirth = user.DateOfBirth,
//                    Bio = user.Bio,
//                    Roles = roles,
//                    IsActive = user.IsActive,
//                    CreatedOn = user.CreatedOn
//                },
//                Permissions = permissions
//            };
//        }

//        public async Task<List<UserPermissionDto>> GetUserPermissionsAsync(Guid userId)
//        {
//            // Get user roles
//            var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(userId);
//            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

//            if (!roleIds.Any())
//                return new List<UserPermissionDto>();

//            // Get all role permissions for these roles
//            var rolePermissions = await _unitOfWork.RolePermissions.AsQueryable()
//                .Include(rp => rp.Module)
//                .Where(rp => roleIds.Contains(rp.RoleId) &&
//                            rp.Module.IsActive &&
//                            !rp.Module.IsDeleted &&
//                            rp.IsActive &&
//                            !rp.IsDeleted)
//                .ToListAsync();

//            // Group by module and aggregate permissions
//            var permissions = rolePermissions
//                .GroupBy(rp => new { rp.ModuleId, rp.Module.Name })
//                .Select(g => new UserPermissionDto
//                {
//                    ModuleName = g.Key.Name,
//                    CanView = g.Any(rp => rp.CanView),
//                    CanAdd = g.Any(rp => rp.CanAdd),
//                    CanEdit = g.Any(rp => rp.CanEdit),
//                    CanDelete = g.Any(rp => rp.CanDelete)
//                })
//                .ToList();

//            return permissions;
//        }

//        private async Task<string> GenerateJwtTokenAsync(User user, IList<string> roles, List<UserPermissionDto> permissions)
//        {
//            if (user == null)
//                throw new ArgumentNullException(nameof(user), "User cannot be null");
//            if (user.Email == null)
//                throw new ArgumentNullException(nameof(user), "User email cannot be null");

//            var tokenHandler = new JwtSecurityTokenHandler();
//            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "MyTemporarySecretKeyForDevelopment12345");

//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                new Claim(ClaimTypes.Email, user.Email),
//                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
//            };

//            // Add all roles as claims
//            foreach (var role in roles)
//            {
//                claims.Add(new Claim(ClaimTypes.Role, role));

//                // Check if this is the SuperAdmin role
//                if (role == "SuperAdmin")
//                {
//                    claims.Add(new Claim("SuperAdmin", "true"));
//                }
//            }

//            // Add module permissions as claims
//            foreach (var permission in permissions)
//            {
//                if (permission.CanView)
//                    claims.Add(new Claim("Permission", $"{permission.ModuleName}:View"));
//                if (permission.CanAdd)
//                    claims.Add(new Claim("Permission", $"{permission.ModuleName}:Add"));
//                if (permission.CanEdit)
//                    claims.Add(new Claim("Permission", $"{permission.ModuleName}:Edit"));
//                if (permission.CanDelete)
//                    claims.Add(new Claim("Permission", $"{permission.ModuleName}:Delete"));
//            }

//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(claims),
//                Expires = DateTime.UtcNow.AddHours(24),
//                SigningCredentials = new SigningCredentials(
//                    new SymmetricSecurityKey(key),
//                    SecurityAlgorithms.HmacSha256Signature),
//                Issuer = _configuration["Jwt:Issuer"],
//                Audience = _configuration["Jwt:Audience"]
//            };

//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            return tokenHandler.WriteToken(token);
//        }
//    }
//}