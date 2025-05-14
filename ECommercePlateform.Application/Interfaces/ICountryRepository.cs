using ECommercePlateform.Domain.Entities;

namespace ECommercePlateform.Application.Interfaces
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        Task<Country> GetCountryWithStatesAsync(Guid id);
        Task<IReadOnlyList<Country>> GetActiveCountriesAsync();
        Task<bool> IsNameUniqueAsync(string name);
        Task<bool> IsCodeUniqueAsync(string code);
        Task<bool> IsNameUniqueAsync(string name, Guid excludeId);
        Task<bool> IsCodeUniqueAsync(string code, Guid excludeId);

    }
}