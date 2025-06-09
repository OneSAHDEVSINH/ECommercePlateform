using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class PermissionRepository(AppDbContext context) : GenericRepository<Permission>(context), IPermissionRepository
    {
        public async Task<Permission> GetByModuleAndTypeAsync(Guid moduleId, string permissionType)
        {
            if (!Enum.TryParse<Permission.PermissionType>(permissionType, out var parsedPermissionType))
            {
                throw new ArgumentException($"Invalid permission type: {permissionType}", nameof(permissionType));
            }

            return await _context.Permissions
                .FirstOrDefaultAsync(p => p.ModuleId == moduleId && p.Type == parsedPermissionType)
                 ?? throw new InvalidOperationException("not found.");
        }
    }
}
