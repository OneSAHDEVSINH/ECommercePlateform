using ECommercePlatform.Application.Interfaces.IAuth;
using ECommercePlatform.Application.Interfaces.ICity;
using ECommercePlatform.Application.Interfaces.ICountry;
using ECommercePlatform.Application.Interfaces.IProduct;
using ECommercePlatform.Application.Interfaces.IState;

namespace ECommercePlatform.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICountryRepository Countries { get; }
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        IStateRepository States { get; }
        ICityRepository Cities { get; }

        // Shall be added more repositories as properties

        Task<int> CompleteAsync();
    }
}