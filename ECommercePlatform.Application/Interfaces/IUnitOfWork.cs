namespace ECommercePlatform.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICountryRepository Countries { get; }
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        IStateRepository States { get; }
        ICityRepository Cities { get; }
        IModuleRepository Modules { get; }
        IPermissionRepository Permissions { get; }
        IRolePermissionRepository RolePermissions { get; }
        IRoleRepository Roles { get; }
        IUserRoleRepository UserRoles { get; }
        // Shall be added more repositories as properties

        Task<int> CompleteAsync();
        Task<int> SaveChangesAsync();
        new void Dispose();
    }
}