﻿using ECommercePlatform.Application.DTOs;
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
                .Include(rp => rp.Module)
                .Include(rp => rp.Role)
                .Where(rp => rp.RoleId == roleId && !rp.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<RolePermission>> GetByModuleIdAsync(Guid moduleId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Module)
                .Where(rp => rp.ModuleId == moduleId && !rp.IsDeleted)
                .ToListAsync();
        }

        public async Task<RolePermission?> GetByRoleAndModuleAsync(Guid roleId, Guid moduleId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Module)
                .Include(rp => rp.Role)
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId &&
                                          rp.ModuleId == moduleId &&
                                          !rp.IsDeleted);
        }

        public async Task DeleteByRoleIdAsync(Guid roleId)
        {
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(rolePermissions);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid roleId, Guid moduleId)
        {
            return await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId &&
                               rp.ModuleId == moduleId &&
                               !rp.IsDeleted);
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
                .Include(rp => rp.Module)
                .Where(rp => rp.IsActive && !rp.IsDeleted)
                .OrderBy(rp => rp.Role.Name)
                .ThenBy(rp => rp.Module.DisplayOrder)
                .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<RolePermission> rolePermissions)
        {
            await _context.RolePermissions.AddRangeAsync(rolePermissions);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRolePermissionsAsync(Guid roleId, List<RoleModulePermissionDto> permissions)
        {
            // Remove existing permissions for the role
            var existingPermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(existingPermissions);

            // Add new permissions
            var newPermissions = permissions.Select(p => RolePermission.Create(
                roleId,
                p.ModuleId,
                p.CanView,
                p.CanAdd,
                p.CanEdit,
                p.CanDelete
            ));

            await _context.RolePermissions.AddRangeAsync(newPermissions);
            await _context.SaveChangesAsync();
        }

        // Updated search function
        private static IQueryable<RolePermission> ApplyRolePermissionSearch(
            IQueryable<RolePermission> query, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return query;

            var searchTerm = searchText.ToLower();
            return query.Where(rp =>
                (rp.Role != null && rp.Role.Name != null &&
                    EF.Functions.Like(rp.Role.Name.ToLower(), $"%{searchTerm}%")) ||
                (rp.Module != null && rp.Module.Name != null &&
                    EF.Functions.Like(rp.Module.Name.ToLower(), $"%{searchTerm}%")));
        }

        // Existing pagination methods remain mostly the same but remove Permission references
        public async Task<PagedResponse<RolePermission>> GetPagedRolePermissionsAsync(
            PagedRequest request,
            Guid? roleId = null,
            Guid? moduleId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            Expression<Func<RolePermission, bool>> baseFilter;

            if (roleId.HasValue && moduleId.HasValue)
            {
                baseFilter = activeOnly
                    ? rp => rp.IsActive && !rp.IsDeleted &&
                           rp.RoleId == roleId.Value &&
                           rp.ModuleId == moduleId.Value
                    : rp => !rp.IsDeleted &&
                           rp.RoleId == roleId.Value &&
                           rp.ModuleId == moduleId.Value;
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
                    ? rp => rp.IsActive && !rp.IsDeleted && rp.ModuleId == moduleId.Value
                    : rp => !rp.IsDeleted && rp.ModuleId == moduleId.Value;
            }
            else
            {
                baseFilter = activeOnly
                    ? rp => rp.IsActive && !rp.IsDeleted
                    : rp => !rp.IsDeleted;
            }

            static IQueryable<RolePermission> searchWithInclude(IQueryable<RolePermission> query, string? searchText)
            {
                var queryWithInclude = query
                    .Include(rp => rp.Role)
                    .Include(rp => rp.Module);

                if (!string.IsNullOrWhiteSpace(searchText))
                    return ApplyRolePermissionSearch(queryWithInclude, searchText);

                return queryWithInclude;
            }

            return await GetPagedAsync(
                request,
                baseFilter,
                searchWithInclude,
                cancellationToken);
        }

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