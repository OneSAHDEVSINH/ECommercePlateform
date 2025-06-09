using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class RolePermissionRepository(AppDbContext context) : GenericRepository<RolePermission>(context), IRolePermissionRepository
    {
        public async Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();
        }

        public async Task DeleteByRoleIdAsync(Guid roleId)
        {
            var rolePermissions = _context.RolePermissions
                .Where(rp => rp.RoleId == roleId);

            _context.RolePermissions.RemoveRange(rolePermissions);
            await _context.SaveChangesAsync();
        }

        Task<List<RolePermission>> IRolePermissionRepository.GetByRoleIdAsync(Guid roleId)
        {
            throw new NotImplementedException();
        }
    }
}
