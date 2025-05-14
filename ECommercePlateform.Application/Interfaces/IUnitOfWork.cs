using ECommercePlateform.Application.Interfaces.IAuth;
using ECommercePlateform.Application.Interfaces.ICity;
using ECommercePlateform.Application.Interfaces.ICountry;
using ECommercePlateform.Application.Interfaces.IProduct;
using ECommercePlateform.Application.Interfaces.IState;

namespace ECommercePlateform.Application.Interfaces
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