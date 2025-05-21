using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces.IGeneral;
using ECommercePlatform.Domain.Entities;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces.ICountry
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        Task<Country> GetCountryWithStatesAsync(Guid id);
        Task<IReadOnlyList<Country>> GetActiveCountriesAsync();
        Task<bool> IsNameUniqueAsync(string name);
        Task<bool> IsCodeUniqueAsync(string code);
        Task<bool> IsNameUniqueAsync(string name, Guid excludeId);
        Task<bool> IsCodeUniqueAsync(string code, Guid excludeId);
        Task<bool> IsNameAndCodeUniqueAsync(string name, string code);
        Task<AppResult<string>> EnsureCodeIsUniqueAsync(string code);
        Task<AppResult<string>> EnsureNameIsUniqueAsync(string name);
        Task<AppResult<string>> EnsureCodeIsUniqueAsync(string code, Guid excludeId);
        Task<AppResult<string>> EnsureNameIsUniqueAsync(string name, Guid excludeId);
        Task<bool> AnyAsync(Expression<Func<Country, bool>> predicate);
    }
}