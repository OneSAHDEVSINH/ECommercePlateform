using ECommercePlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ECommercePlatform.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICountryRepository Countries { get; }
        IUserRepository Users { get; }
        IStateRepository States { get; }
        ICityRepository Cities { get; }
        IModuleRepository Modules { get; }
        IRolePermissionRepository RolePermissions { get; }
        IRoleRepository Roles { get; }
        IUserRoleRepository UserRoles { get; }
        UserManager<User> UserManager { get; }
        RoleManager<Role> RoleManager { get; }
        SignInManager<User> SignInManager { get; }
        // Shall be added more repositories as properties

        Task<int> CompleteAsync();
        Task<int> SaveChangesAsync();
        new void Dispose();
    }
}