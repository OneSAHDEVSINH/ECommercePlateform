using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : GenericRepository<User>(context), IUserRepository
    {
        public async Task<User?> FindUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User?> FindUserByEmailAndPasswordAsync(string email, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password && !u.IsDeleted);
        }

        public async Task<User?> FindUserWithRolesByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _context.Users
                .AnyAsync(u => u.Email != null &&
                              u.Email.ToLower().Trim() == email.ToLower().Trim() &&
                              !u.IsDeleted);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, Guid excludeUserId)
        {
            return !await _context.Users
                .AnyAsync(u => u.Email != null &&
                              u.Email.ToLower().Trim() == email.ToLower().Trim() &&
                              u.Id != excludeUserId &&
                              !u.IsDeleted);
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

        public async Task<bool> AnyAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.AnyAsync(predicate);
        }

        public IQueryable<User> AsQueryable()
        {
            return _context.Users.AsQueryable();
        }

        public async Task DeleteByUserIdAsync(Guid userId)
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            _context.UserRoles.RemoveRange(userRoles);
            await _context.SaveChangesAsync();
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
            CancellationToken cancellationToken = default)
        {
            // Create base filter
            Expression<Func<User, bool>> baseFilter = activeOnly
                ? u => u.IsActive && !u.IsDeleted
                : u => !u.IsDeleted;

            // Define a search function that also includes roles
            static IQueryable<User> searchWithInclude(IQueryable<User> query, string? searchText)
            {
                // First include related entities
                var queryWithInclude = query
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role);

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

        // Get paginated user DTOs
        public async Task<PagedResponse<UserDto>> GetPagedUserDtosAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            var pagedEntities = await GetPagedUsersAsync(
                request,
                activeOnly,
                cancellationToken);

            // Map entities to DTOs
            var dtos = pagedEntities.Items.Select(u => (UserDto)u).ToList();

            return new PagedResponse<UserDto>
            {
                Items = dtos,
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
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