using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;
using ECommercePlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class PermissionRepository(AppDbContext context) : GenericRepository<Permission>(context), IPermissionRepository
    {
        public async Task<Permission?> GetByModuleAndTypeAsync(Guid moduleId, string permissionType)
        {
            if (!Enum.TryParse<PermissionType>(permissionType, out var parsedPermissionType))
            {
                throw new ArgumentException($"Invalid permission type: {permissionType}", nameof(permissionType));
            }

            return await _context.Permissions
                .Include(p => p.Module)
                .FirstOrDefaultAsync(p => p.ModuleId == moduleId &&
                                         p.Type == parsedPermissionType &&
                                         !p.IsDeleted);
        }

        public async Task<List<Permission>> GetByModuleIdAsync(Guid moduleId)
        {
            return await _context.Permissions
                .Include(p => p.Module)
                .Where(p => p.ModuleId == moduleId && !p.IsDeleted)
                .OrderBy(p => p.Type)
                .ToListAsync();
        }

        public new async Task<List<Permission>> GetAllAsync()
        {
            return await _context.Permissions
                .Include(p => p.Module)
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public new async Task<Permission?> GetByIdAsync(Guid id)
        {
            return await _context.Permissions
                .Include(p => p.Module)
                .Include(p => p.RolePermissions)
                    .ThenInclude(rp => rp.Role)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<bool> IsNameUniqueInModuleAsync(string name, Guid moduleId)
        {
            return !await _context.Permissions
                .AnyAsync(p => p.Name != null &&
                              p.Name.ToLower().Trim() == name.ToLower().Trim() &&
                              p.ModuleId == moduleId &&
                              !p.IsDeleted);
        }

        public async Task<bool> IsNameUniqueInModuleAsync(string name, Guid moduleId, Guid excludeId)
        {
            return !await _context.Permissions
                .AnyAsync(p => p.Name != null &&
                              p.Name.ToLower().Trim() == name.ToLower().Trim() &&
                              p.ModuleId == moduleId &&
                              p.Id != excludeId &&
                              !p.IsDeleted);
        }

        public async Task<List<Permission>> GetActivePermissionsAsync()
        {
            return await _context.Permissions
                .Include(p => p.Module)
                .Where(p => p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.Module.DisplayOrder)
                .ThenBy(p => p.Type)
                .ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<Permission, bool>> predicate)
        {
            return await _context.Permissions.AnyAsync(predicate);
        }

        public IQueryable<Permission> AsQueryable()
        {
            return _context.Permissions.AsQueryable();
        }

        // Combined validation method that returns a Result object
        public Task<Result<(string name, Guid moduleId)>> EnsureNameIsUniqueInModuleAsync(
            string name, Guid moduleId, Guid? excludeId = null)
        {
            return Result.Success((name, moduleId))
                // Validate name is not empty
                .Ensure(tuple => !string.IsNullOrEmpty(tuple.name?.Trim()),
                    "Permission name cannot be null or empty.")
                // Normalize the input
                .Map(tuple => (tuple.name.Trim().ToLower(), tuple.moduleId))
                // Check uniqueness against database
                .Bind(async normalized =>
                {
                    var query = _context.Permissions.Where(p =>
                        p.Name != null &&
                        p.Name.ToLower().Trim() == normalized.Item1 &&
                        p.ModuleId == normalized.moduleId &&
                        !p.IsDeleted);

                    // Apply ID exclusion if provided
                    if (excludeId.HasValue)
                        query = query.Where(p => p.Id != excludeId.Value);

                    var exists = await query.AnyAsync();

                    return exists
                        ? Result.Failure<(string, Guid)>($"Permission with name \"{name}\" already exists for this module.")
                        : Result.Success((normalized.Item1, normalized.moduleId));
                });
        }

        // Search function for permissions
        private static IQueryable<Permission> ApplyPermissionSearch(IQueryable<Permission> query, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return query;

            var searchTerm = searchText.ToLower();
            return query.Where(p =>
                (p.Name != null && EF.Functions.Like(p.Name.ToLower(), $"%{searchTerm}%")) ||
                (p.Description != null && EF.Functions.Like(p.Description.ToLower(), $"%{searchTerm}%")) ||
                (p.Module != null && p.Module.Name != null &&
                    EF.Functions.Like(p.Module.Name.ToLower(), $"%{searchTerm}%")));
        }

        // Get paginated permissions with optional module filter
        public async Task<PagedResponse<Permission>> GetPagedPermissionsAsync(
            PagedRequest request,
            Guid? moduleId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            // Create base filter
            Expression<Func<Permission, bool>> baseFilter;

            if (moduleId.HasValue)
            {
                baseFilter = activeOnly
                    ? p => p.IsActive && !p.IsDeleted && p.ModuleId == moduleId.Value
                    : p => !p.IsDeleted && p.ModuleId == moduleId.Value;
            }
            else
            {
                baseFilter = activeOnly
                    ? p => p.IsActive && !p.IsDeleted
                    : p => !p.IsDeleted;
            }

            // Define a search function that also includes modules
            static IQueryable<Permission> searchWithInclude(IQueryable<Permission> query, string? searchText)
            {
                // First include related entities
                var queryWithInclude = query
                    .Include(p => p.Module);

                // Then apply search if text is provided
                if (!string.IsNullOrWhiteSpace(searchText))
                    return ApplyPermissionSearch(queryWithInclude, searchText);

                return queryWithInclude;
            }

            // Use the generic paging method
            return await GetPagedAsync(
                request,
                baseFilter,
                searchWithInclude,
                cancellationToken);
        }

        // Get paginated permission DTOs
        public async Task<PagedResponse<PermissionDto>> GetPagedPermissionDtosAsync(
            PagedRequest request,
            Guid? moduleId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            var pagedEntities = await GetPagedPermissionsAsync(
                request,
                moduleId,
                activeOnly,
                cancellationToken);

            // Map entities to DTOs
            var dtos = pagedEntities.Items.Select(p => (PermissionDto)p).ToList();

            return new PagedResponse<PermissionDto>
            {
                Items = dtos,
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }

        // Get paginated permission list DTOs (simplified version for lists)
        public async Task<PagedResponse<PermissionListDto>> GetPagedPermissionListDtosAsync(
            PagedRequest request,
            Guid? moduleId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            var pagedEntities = await GetPagedPermissionsAsync(
                request,
                moduleId,
                activeOnly,
                cancellationToken);

            // Map entities to list DTOs
            var dtos = pagedEntities.Items.Select(p => (PermissionListDto)p).ToList();

            return new PagedResponse<PermissionListDto>
            {
                Items = dtos,
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }
    }
}