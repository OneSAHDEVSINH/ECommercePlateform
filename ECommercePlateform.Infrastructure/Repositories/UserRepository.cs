using ECommercePlateform.Application.Interfaces.IAuth;
using ECommercePlateform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommercePlateform.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User> FindUserByEmailAndPasswordAsync(string email, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.IsActive && !u.IsDeleted)
                ?? throw new InvalidOperationException("Email or password Invalid");
        }

        public async Task<User> FindUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive && !u.IsDeleted)
                ?? throw new InvalidOperationException("User not found.");

            return user!;
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}