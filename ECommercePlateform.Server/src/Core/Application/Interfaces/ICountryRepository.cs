using ECommercePlateform.Server.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommercePlateform.Server.Core.Application.Interfaces
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        Task<Country> GetCountryWithStatesAsync(Guid id);
        Task<IReadOnlyList<Country>> GetActiveCountriesAsync();
    }
} 