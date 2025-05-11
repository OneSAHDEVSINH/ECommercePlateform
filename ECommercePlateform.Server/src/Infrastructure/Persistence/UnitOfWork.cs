using ECommercePlateform.Server.Core.Application.Interfaces;
using ECommercePlateform.Server.Infrastructure.Persistence.Repositories;
using System.Threading.Tasks;

namespace ECommercePlateform.Server.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private ICountryRepository _countryRepository;
        private IUserRepository _userRepository;
        private IProductRepository _productRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public ICountryRepository Countries => _countryRepository ??= new CountryRepository(_context);

        public IUserRepository Users => _userRepository ??= new UserRepository(_context);

        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);

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