using ECommercePlateform.Server.Core.Application.Interfaces;
using ECommercePlateform.Server.Core.Domain.Entities;

namespace ECommercePlateform.Server.src.Core.Application.Interfaces
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
