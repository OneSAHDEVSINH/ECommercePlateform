using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;

namespace ECommercePlatform.Infrastructure.Repositories
{
    public class ModuleRepository(AppDbContext context) : GenericRepository<Module>(context), IModuleRepository
    {
    }
}
