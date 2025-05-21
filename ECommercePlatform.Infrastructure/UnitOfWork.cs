using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.ICity;
using ECommercePlatform.Application.Interfaces.ICountry;
using ECommercePlatform.Application.Interfaces.IProduct;
using ECommercePlatform.Application.Interfaces.IState;
using ECommercePlatform.Application.Interfaces.IUserAuth;
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

        public ICountryRepository Countries => _countryRepository ??= new CountryRepository(_context);

        public IUserRepository Users => _userRepository ??= new UserRepository(_context);

        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);

        public IStateRepository States => _stateRepository ??= new StateRepository(_context);

        public ICityRepository Cities => _cityRepository ??= new CityRepository(_context);

        public async Task<int> CompleteAsync()
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