using ECommercePlatform.Server.Core.Application.Interfaces;
using ECommercePlatform.Server.Core.Domain.Entities;

namespace ECommercePlatform.Server.src.Core.Application.Interfaces
{
    public interface ICityRepository : IGenericRepository<City>
    {
        Task<City> GetCityByIdAsync(Guid id);
        Task<IReadOnlyList<City>> GetActiveCountries();
        Task<IReadOnlyList<City>> GetCitiesByStateIdAsync(Guid stateId);
        Task<bool> IsNameUniqueInStateAsync(string name, Guid stateId);
        Task<bool> IsNameUniqueInStateAsync(string name, Guid stateId, Guid excludeId);

    }
}
