using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Helpers;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context, ISuperAdminService superAdminService, ICurrentUserService currentUserService) : GenericRepository<User>(context), IUserRepository
    {
        private readonly ISuperAdminService _superAdminService = superAdminService;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<User?> FindUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public new async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Module)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public new async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersByRoleIdAsync(Guid roleId)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .Where(u => u.UserRoles.Any(ur => ur.RoleId == roleId) && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<User>> GetActiveUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.IsActive && !u.IsDeleted)
                .ToListAsync();
        }

        // Combined validation method that returns a Result object
        public Task<Result<string>> EnsureEmailIsUniqueAsync(string email, Guid? excludeId = null)
        {
            return Result.Success(email)
                // Validate email is not empty
                .Ensure(e => !string.IsNullOrEmpty(e?.Trim()), "Email cannot be null or empty.")
                // Normalize the input
                .Map(e => e.Trim().ToLower())
                // Check uniqueness against database
                .Bind(async normalizedEmail =>
                {
                    var query = _context.Users.Where(u =>
                        u.Email != null &&
                        u.Email.ToLower().Trim() == normalizedEmail &&
                        !u.IsDeleted);

                    // Apply ID exclusion if provided
                    if (excludeId.HasValue)
                        query = query.Where(u => u.Id != excludeId.Value);

                    var exists = await query.AnyAsync();

                    return exists
                        ? Result.Failure<string>($"User with email \"{email}\" already exists.")
                        : Result.Success(normalizedEmail);
                });
        }

        // Combined validation method that returns a Result object
        public Task<Result<string>> EnsurePhoneIsUniqueAsync(string phone, Guid? excludeId = null)
        {
            return Result.Success(phone)
                // Validate email is not empty
                .Ensure(e => !string.IsNullOrEmpty(e?.Trim()), "Phone Number cannot be null or empty.")
                // Normalize the input
                .Map(e => e.Trim().ToLower())
                // Check uniqueness against database
                .Bind(async normalizedPhone =>
                {
                    var query = _context.Users.Where(u =>
                        u.PhoneNumber != null &&
                        u.PhoneNumber.ToLower().Trim() == normalizedPhone &&
                        !u.IsDeleted);

                    // Apply ID exclusion if provided
                    if (excludeId.HasValue)
                        query = query.Where(u => u.Id != excludeId.Value);

                    var exists = await query.AnyAsync();

                    return exists
                        ? Result.Failure<string>($"User with phone number \"{phone}\" already exists.")
                        : Result.Success(normalizedPhone);
                });
        }

        // Search function for users
        private static IQueryable<User> ApplyUserSearch(IQueryable<User> query, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return query;

            var searchTerm = searchText.ToLower();
            return query.Where(u =>
                (u.FirstName != null && EF.Functions.Like(u.FirstName.ToLower(), $"%{searchTerm}%")) ||
                (u.LastName != null && EF.Functions.Like(u.LastName.ToLower(), $"%{searchTerm}%")) ||
                (u.Email != null && EF.Functions.Like(u.Email.ToLower(), $"%{searchTerm}%")) ||
                (u.PhoneNumber != null && EF.Functions.Like(u.PhoneNumber, $"%{searchTerm}%")));
        }

        // Get paginated users
        public async Task<PagedResponse<User>> GetPagedUsersAsync(
    PagedRequest request,
    bool activeOnly = true,
    bool includeRoles = true,
    Guid? roleId = null,
    CancellationToken cancellationToken = default)
        {
            // Create base filter
            Expression<Func<User, bool>> baseFilter;

            // Check if current user is the superadmin
            var currentUserIdStr = _currentUserService.UserId;
            var isSuperAdminViewing = false;

            if (!string.IsNullOrEmpty(currentUserIdStr) && Guid.TryParse(currentUserIdStr, out var currentUserId))
            {
                var currentUser = await _context.Users.FindAsync(currentUserId);
                if (currentUser != null)
                {
                    isSuperAdminViewing = _superAdminService.IsSuperAdminEmail(currentUser.Email);
                }
            }

            // If not viewed by superadmin, exclude the superadmin account from results
            if (!isSuperAdminViewing)
            {
                var superAdminEmail = "admin@admin.com"; // This should match the configured superadmin email
                baseFilter = activeOnly
                    ? u => u.IsActive && !u.IsDeleted && u.Email != superAdminEmail
                    : u => !u.IsDeleted && u.Email != superAdminEmail;
            }

            if (roleId.HasValue)
            {
                baseFilter = activeOnly
                    ? u => u.IsActive && !u.IsDeleted && u.UserRoles.Any(ur => ur.RoleId == roleId.Value)
                    : u => !u.IsDeleted && u.UserRoles.Any(ur => ur.RoleId == roleId.Value);
            }
            else
            {
                baseFilter = activeOnly
                    ? u => u.IsActive && !u.IsDeleted
                    : u => !u.IsDeleted;
            }

            // Apply date filtering if provided
            if (request.StartDate.HasValue)
            {
                var startDate = request.StartDate.Value;
                baseFilter = baseFilter.And(m => m.CreatedOn >= startDate);
            }

            if (request.EndDate.HasValue)
            {
                var endDate = request.EndDate.Value;
                baseFilter = baseFilter.And(m => m.CreatedOn <= endDate);
            }

            // Define a search function that also includes roles regardless of search text
            IQueryable<User> searchWithInclude(IQueryable<User> query, string? searchText)
            {
                // Always include related entities if includeRoles is true
                var queryWithInclude = includeRoles
                    ? query.Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    : query;

                // Then apply search if text is provided
                if (!string.IsNullOrWhiteSpace(searchText))
                    return ApplyUserSearch(queryWithInclude, searchText);

                return queryWithInclude;
            }

            // Use the generic paging method
            return await GetPagedAsync(
                request,
                baseFilter,
                searchWithInclude,
                cancellationToken);
        }

        public async Task<PagedResponse<UserDto>> GetPagedUserDtosAsync(
    PagedRequest request,
    bool activeOnly = true,
    bool includeRoles = true,
    Guid? roleId = null,
    CancellationToken cancellationToken = default)
        {
            var pagedEntities = await GetPagedUsersAsync(
                request,
                activeOnly,
                includeRoles,
                roleId,
                cancellationToken);

            // Explicitly load roles if we need them
            if (includeRoles)
            {
                var userIds = pagedEntities.Items.Select(u => u.Id).ToList();

                // Eagerly load all roles for these users to ensure they're properly populated
                var userRolesWithRoles = await _context.UserRoles
                    .Include(ur => ur.Role)
                    .Where(ur => userIds.Contains(ur.UserId) && !ur.IsDeleted && ur.IsActive && ur.Role.IsActive && !ur.Role.IsDeleted)
                    .ToListAsync(cancellationToken);

                // Group by user ID for easy lookup
                var userRolesLookup = userRolesWithRoles
                    .GroupBy(ur => ur.UserId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Create DTOs with explicit role loading
                var dtos = pagedEntities.Items.Select(user =>
                {
                    var dto = (UserDto)user;

                    // If roles should be included but are missing, use our lookup
                    if (userRolesLookup.TryGetValue(user.Id, out var userRoles) && (dto.Roles == null || !dto.Roles.Any()))
                    {
                        dto = new UserDto
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            Gender = user.Gender,
                            DateOfBirth = user.DateOfBirth,
                            Bio = user.Bio,
                            IsActive = user.IsActive,
                            CreatedOn = user.CreatedOn,
                            Roles = userRoles
                                .Where(ur => ur.Role != null)
                                .Select(ur => (RoleDto)ur.Role!)
                                .ToList()
                        };
                    }

                    return dto;
                }).ToList();

                return new PagedResponse<UserDto>
                {
                    Items = dtos,
                    TotalCount = pagedEntities.TotalCount,
                    PageNumber = pagedEntities.PageNumber,
                    PageSize = pagedEntities.PageSize
                };
            }
            else
            {
                // Basic mapping without roles
                var dtos = pagedEntities.Items.Select(u => (UserDto)u).ToList();
                return new PagedResponse<UserDto>
                {
                    Items = dtos,
                    TotalCount = pagedEntities.TotalCount,
                    PageNumber = pagedEntities.PageNumber,
                    PageSize = pagedEntities.PageSize
                };
            }
        }

        // Get paginated user list DTOs (simplified version for lists)
        public async Task<PagedResponse<UserListDto>> GetPagedUserListDtosAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            var pagedEntities = await GetPagedUsersAsync(
                request,
                activeOnly,
                includeRoles: false,
                roleId: null, // Explicitly pass null for the roleId parameter
                cancellationToken);

            // Map entities to list DTOs
            var dtos = pagedEntities.Items.Select(u => (UserListDto)u).ToList();

            return new PagedResponse<UserListDto>
            {
                Items = dtos,
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }
    }
}