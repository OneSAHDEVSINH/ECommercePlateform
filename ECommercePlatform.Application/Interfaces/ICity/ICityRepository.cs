using ECommercePlatform.Application.Interfaces.IGeneral;
using ECommercePlatform.Domain.Entities;

namespace ECommercePlatform.Application.Interfaces.ICity
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
