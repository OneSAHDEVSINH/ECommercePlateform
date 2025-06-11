using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class ModuleRepository(AppDbContext context) : GenericRepository<Module>(context), IModuleRepository
    {
        public async Task<bool> IsNameUniqueAsync(string name)
        {
            return !await _context.Modules
                .AnyAsync(m => m.Name != null &&
                              m.Name.ToLower().Trim() == name.ToLower().Trim() &&
                              !m.IsDeleted);
        }

        public async Task<bool> IsNameUniqueAsync(string name, Guid excludeId)
        {
            return !await _context.Modules
                .AnyAsync(m => m.Name != null &&
                              m.Name.ToLower().Trim() == name.ToLower().Trim() &&
                              m.Id != excludeId &&
                              !m.IsDeleted);
        }

        public async Task<bool> IsRouteUniqueAsync(string route)
        {
            return !await _context.Modules
                .AnyAsync(m => m.Route != null &&
                              m.Route.ToLower().Trim() == route.ToLower().Trim() &&
                              !m.IsDeleted);
        }

        public async Task<bool> IsRouteUniqueAsync(string route, Guid excludeId)
        {
            return !await _context.Modules
                .AnyAsync(m => m.Route != null &&
                              m.Route.ToLower().Trim() == route.ToLower().Trim() &&
                              m.Id != excludeId &&
                              !m.IsDeleted);
        }

        public new async Task<Module?> GetByIdAsync(Guid id)
        {
            return await _context.Modules
                .Include(m => m.Permissions)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        }

        public new async Task<List<Module>> GetAllAsync()
        {
            return await _context.Modules
                .Include(m => m.Permissions)
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<Module>> GetActiveModulesAsync()
        {
            return await _context.Modules
                .Include(m => m.Permissions)
                .Where(m => m.IsActive && !m.IsDeleted)
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync();
        }

        public async Task<Module?> GetByRouteAsync(string route)
        {
            return await _context.Modules
                .Include(m => m.Permissions)
                .FirstOrDefaultAsync(m => m.Route != null &&
                                         m.Route.ToLower() == route.ToLower() &&
                                         !m.IsDeleted);
        }

        public async Task<bool> AnyAsync(Expression<Func<Module, bool>> predicate)
        {
            return await _context.Modules.AnyAsync(predicate);
        }

        public IQueryable<Module> AsQueryable()
        {
            return _context.Modules.AsQueryable();
        }

        // Combined validation method for name uniqueness
        public Task<Result<string>> EnsureNameIsUniqueAsync(string name, Guid? excludeId = null)
        {
            return Result.Success(name)
                // Validate name is not empty
                .Ensure(n => !string.IsNullOrEmpty(n?.Trim()), "Module name cannot be null or empty.")
                // Normalize the input
                .Map(n => n.Trim().ToLower())
                // Check uniqueness against database
                .Bind(async normalizedName =>
                {
                    var query = _context.Modules.Where(m =>
                        m.Name != null &&
                        m.Name.ToLower().Trim() == normalizedName &&
                        !m.IsDeleted);

                    // Apply ID exclusion if provided
                    if (excludeId.HasValue)
                        query = query.Where(m => m.Id != excludeId.Value);

                    var exists = await query.AnyAsync();

                    return exists
                        ? Result.Failure<string>($"Module with name \"{name}\" already exists.")
                        : Result.Success(normalizedName);
                });
        }

        // Combined validation method for route uniqueness
        public Task<Result<string>> EnsureRouteIsUniqueAsync(string route, Guid? excludeId = null)
        {
            return Result.Success(route)
                // Validate route is not empty
                .Ensure(r => !string.IsNullOrEmpty(r?.Trim()), "Module route cannot be null or empty.")
                // Normalize the input
                .Map(r => r.Trim().ToLower())
                // Check uniqueness against database
                .Bind(async normalizedRoute =>
                {
                    var query = _context.Modules.Where(m =>
                        m.Route != null &&
                        m.Route.ToLower().Trim() == normalizedRoute &&
                        !m.IsDeleted);

                    // Apply ID exclusion if provided
                    if (excludeId.HasValue)
                        query = query.Where(m => m.Id != excludeId.Value);

                    var exists = await query.AnyAsync();

                    return exists
                        ? Result.Failure<string>($"Module with route \"{route}\" already exists.")
                        : Result.Success(normalizedRoute);
                });
        }

        // Search function for modules
        private static IQueryable<Module> ApplyModuleSearch(IQueryable<Module> query, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return query;

            var searchTerm = searchText.ToLower();
            return query.Where(m =>
                (m.Name != null && EF.Functions.Like(m.Name.ToLower(), $"%{searchTerm}%")) ||
                (m.Description != null && EF.Functions.Like(m.Description.ToLower(), $"%{searchTerm}%")) ||
                (m.Route != null && EF.Functions.Like(m.Route.ToLower(), $"%{searchTerm}%")));
        }

        // Get paginated modules
        public async Task<PagedResponse<Module>> GetPagedModulesAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            // Create base filter
            Expression<Func<Module, bool>> baseFilter = activeOnly
                ? m => m.IsActive && !m.IsDeleted
                : m => !m.IsDeleted;

            // Define a search function that also includes permissions
            static IQueryable<Module> searchWithInclude(IQueryable<Module> query, string? searchText)
            {
                // First include related entities
                var queryWithInclude = query
                    .Include(m => m.Permissions);

                // Then apply search if text is provided
                if (!string.IsNullOrWhiteSpace(searchText))
                    return ApplyModuleSearch(queryWithInclude, searchText);

                return queryWithInclude;
            }

            // Use the generic paging method
            return await GetPagedAsync(
                request,
                baseFilter,
                searchWithInclude,
                cancellationToken);
        }

        // Get paginated module DTOs
        public async Task<PagedResponse<ModuleDto>> GetPagedModuleDtosAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            var pagedEntities = await GetPagedModulesAsync(
                request,
                activeOnly,
                cancellationToken);

            // Map entities to DTOs
            var dtos = pagedEntities.Items.Select(m => (ModuleDto)m).ToList();

            return new PagedResponse<ModuleDto>
            {
                Items = dtos,
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }

        // Get paginated module list DTOs (simplified version for lists)
        public async Task<PagedResponse<ModuleListDto>> GetPagedModuleListDtosAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            var pagedEntities = await GetPagedModulesAsync(
                request,
                activeOnly,
                cancellationToken);

            // Map entities to list DTOs
            var dtos = pagedEntities.Items.Select(m => (ModuleListDto)m).ToList();

            return new PagedResponse<ModuleListDto>
            {
                Items = dtos,
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }
    }
}