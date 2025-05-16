using ECommercePlatform.Server.Core.Application.Interfaces;
using ECommercePlatform.Server.Core.Domain.Entities;

namespace ECommercePlatform.Server.src.Core.Application.Interfaces
{
    public interface IStateRepository : IGenericRepository<State>
    {
        Task<State> GetStateWithCitiesAsync(Guid id);
        Task<IReadOnlyList<State>> GetActiveStatesAsync();
        Task<IReadOnlyList<State>> GetStatesByCountryIdAsync(Guid countryId);
        Task<bool> IsNameUniqueInCountryAsync(string name, Guid countryId);
        Task<bool> IsCodeUniqueInCountryAsync(string code, Guid countryId);
        Task<bool> IsNameUniqueInCountryAsync(string name, Guid countryId, Guid excludeId);
        Task<bool> IsCodeUniqueInCountryAsync(string code, Guid countryId, Guid excludeId);

    }
}
