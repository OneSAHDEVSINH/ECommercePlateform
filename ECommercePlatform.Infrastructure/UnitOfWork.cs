using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Infrastructure.Repositories;

namespace ECommercePlatform.Infrastructure
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context = context;
        private ICountryRepository? _countryRepository;
        private IUserRepository? _userRepository;
        private IProductRepository? _productRepository;
        private IStateRepository? _stateRepository;
        private ICityRepository? _cityRepository;
        private IRolePermissionRepository? _rolePermissionRepository;
        private IModuleRepository? _moduleRepository;
        private IPermissionRepository? _permissionRepository;
        private IRoleRepository? _roleRepository;
        private IUserRoleRepository? _userRoleRepository;

        public ICountryRepository Countries => _countryRepository ??= new CountryRepository(_context);

        public IUserRepository Users => _userRepository ??= new UserRepository(_context);

        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);

        public IStateRepository States => _stateRepository ??= new StateRepository(_context);

        public ICityRepository Cities => _cityRepository ??= new CityRepository(_context);

        public IRolePermissionRepository RolePermissions => _rolePermissionRepository ??= new RolePermissionRepository(_context);

        public IModuleRepository Modules => _moduleRepository ??= new ModuleRepository(_context);

        public IPermissionRepository Permissions => _permissionRepository ??= new PermissionRepository(_context);

        public IRoleRepository Roles => _roleRepository ??= new RoleRepository(_context);

        public IUserRoleRepository UserRoles => _userRoleRepository ??= new UserRoleRepository(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}