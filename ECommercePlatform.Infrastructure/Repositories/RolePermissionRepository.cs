using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class RolePermissionRepository(AppDbContext context) : GenericRepository<RolePermission>(context), IRolePermissionRepository
    {
        public async Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Permission)
                    .ThenInclude(p => p.Module)
                .Include(rp => rp.Role)
                .Where(rp => rp.RoleId == roleId && !rp.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<RolePermission>> GetByPermissionIdAsync(Guid permissionId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .Where(rp => rp.PermissionId == permissionId && !rp.IsDeleted)
                .ToListAsync();
        }

        public async Task DeleteByRoleIdAsync(Guid roleId)
        {
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(rolePermissions);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid roleId, Guid permissionId)
        {
            return await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId &&
                               rp.PermissionId == permissionId &&
                               !rp.IsDeleted);
        }

        public async Task<List<RolePermission>> GetByRoleIdAndModuleIdAsync(Guid roleId, Guid moduleId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId &&
                           rp.Permission.ModuleId == moduleId &&
                           !rp.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<RolePermission, bool>> predicate)
        {
            return await _context.RolePermissions.AnyAsync(predicate);
        }

        public IQueryable<RolePermission> AsQueryable()
        {
            return _context.RolePermissions.AsQueryable();
        }

        public async Task<List<RolePermission>> GetActiveRolePermissionsAsync()
        {
            return await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                    .ThenInclude(p => p.Module)
                .Where(rp => rp.IsActive && !rp.IsDeleted)
                .OrderBy(rp => rp.Role.Name)
                .ToListAsync();
        }

        // Search function for role permissions
        private static IQueryable<RolePermission> ApplyRolePermissionSearch(
            IQueryable<RolePermission> query, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return query;

            var searchTerm = searchText.ToLower();
            return query.Where(rp =>
                (rp.Role != null && rp.Role.Name != null &&
                    EF.Functions.Like(rp.Role.Name.ToLower(), $"%{searchTerm}%")) ||
                (rp.Permission != null && rp.Permission.Name != null &&
                    EF.Functions.Like(rp.Permission.Name.ToLower(), $"%{searchTerm}%")) ||
                (rp.Permission != null && rp.Permission.Module != null &&
                    rp.Permission.Module.Name != null &&
                    EF.Functions.Like(rp.Permission.Module.Name.ToLower(), $"%{searchTerm}%")));
        }

        // Get paginated role permissions with filters
        public async Task<PagedResponse<RolePermission>> GetPagedRolePermissionsAsync(
            PagedRequest request,
            Guid? roleId = null,
            Guid? moduleId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            // Create base filter based on provided parameters
            Expression<Func<RolePermission, bool>> baseFilter;

            if (roleId.HasValue && moduleId.HasValue)
            {
                baseFilter = activeOnly
                    ? rp => rp.IsActive && !rp.IsDeleted &&
                           rp.RoleId == roleId.Value &&
                           rp.Permission.ModuleId == moduleId.Value
                    : rp => !rp.IsDeleted &&
                           rp.RoleId == roleId.Value &&
                           rp.Permission.ModuleId == moduleId.Value;
            }
            else if (roleId.HasValue)
            {
                baseFilter = activeOnly
                    ? rp => rp.IsActive && !rp.IsDeleted && rp.RoleId == roleId.Value
                    : rp => !rp.IsDeleted && rp.RoleId == roleId.Value;
            }
            else if (moduleId.HasValue)
            {
                baseFilter = activeOnly
                    ? rp => rp.IsActive && !rp.IsDeleted && rp.Permission.ModuleId == moduleId.Value
                    : rp => !rp.IsDeleted && rp.Permission.ModuleId == moduleId.Value;
            }
            else
            {
                baseFilter = activeOnly
                    ? rp => rp.IsActive && !rp.IsDeleted
                    : rp => !rp.IsDeleted;
            }

            // Define a search function that includes related entities
            static IQueryable<RolePermission> searchWithInclude(IQueryable<RolePermission> query, string? searchText)
            {
                // First include related entities
                var queryWithInclude = query
                    .Include(rp => rp.Role)
                    .Include(rp => rp.Permission)
                        .ThenInclude(p => p.Module);

                // Then apply search if text is provided
                if (!string.IsNullOrWhiteSpace(searchText))
                    return ApplyRolePermissionSearch(queryWithInclude, searchText);

                return queryWithInclude;
            }

            // Use the generic paging method
            return await GetPagedAsync(
                request,
                baseFilter,
                searchWithInclude,
                cancellationToken);
        }

        // Get paginated role permission DTOs
        public async Task<PagedResponse<RolePermissionDto>> GetPagedRolePermissionDtosAsync(
            PagedRequest request,
            Guid? roleId = null,
            Guid? moduleId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            var pagedEntities = await GetPagedRolePermissionsAsync(
                request,
                roleId,
                moduleId,
                activeOnly,
                cancellationToken);

            // Map entities to DTOs
            var dtos = pagedEntities.Items.Select(rp => (RolePermissionDto)rp).ToList();

            return new PagedResponse<RolePermissionDto>
            {
                Items = dtos,
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }
    }
}