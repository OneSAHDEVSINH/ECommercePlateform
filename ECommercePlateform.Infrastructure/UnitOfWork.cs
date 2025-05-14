using ECommercePlateform.Application.Interfaces;
using ECommercePlateform.Application.Interfaces.IAuth;
using ECommercePlateform.Application.Interfaces.ICity;
using ECommercePlateform.Application.Interfaces.ICountry;
using ECommercePlateform.Application.Interfaces.IProduct;
using ECommercePlateform.Application.Interfaces.IState;
using ECommercePlateform.Infrastructure.Repositories;

namespace ECommercePlateform.Infrastructure
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
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
        }
    }
}