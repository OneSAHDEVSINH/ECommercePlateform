using ECommercePlatform.Server.src.Core.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace ECommercePlatform.Server.Core.Application.Interfaces
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