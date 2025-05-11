using System;
using System.Threading.Tasks;

namespace ECommercePlateform.Server.Core.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICountryRepository Countries { get; }
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        // Add more repositories as properties

        Task<int> CompleteAsync();
    }
} 