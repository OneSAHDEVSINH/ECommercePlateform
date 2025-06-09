using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class UserRoleRepository(AppDbContext context) : GenericRepository<UserRole>(context), IUserRoleRepository
    {
        public async Task<List<UserRole>> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();
        }

        public async Task DeleteByUserIdAsync(Guid userId)
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            _context.UserRoles.RemoveRange(userRoles);
        }
    }
}
