using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class RoleRepository(AppDbContext context) : GenericRepository<Role>(context), IRoleRepository
    {
    }
}
