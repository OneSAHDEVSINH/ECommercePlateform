using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces.IGeneral;
using ECommercePlatform.Domain.Entities;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces.IState
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
        Task<AppResult<string>> EnsureCodeIsUniqueAsync(string code);
        Task<AppResult<string>> EnsureNameIsUniqueAsync(string name);
        Task<AppResult<string>> EnsureCodeIsUniqueAsync(string code, Guid excludeId);
        Task<AppResult<string>> EnsureNameIsUniqueAsync(string name, Guid excludeId);
        Task<AppResult<string>> EnsureNameIsUniqueInCountryAsync(string name, Guid countryId);
        Task<AppResult<string>> EnsureCodeIsUniqueInCountryAsync(string name, Guid countryId);
        Task<bool> AnyAsync(Expression<Func<Country, bool>> predicate);

    }
}
