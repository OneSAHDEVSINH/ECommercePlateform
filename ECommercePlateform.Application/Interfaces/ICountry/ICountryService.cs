using ECommercePlateform.Application.DTOs;

namespace ECommercePlateform.Application.Interfaces.ICountry
{
    public interface ICountryService
    {
        Task<IReadOnlyList<CountryDto>> GetAllCountriesAsync();
        Task<CountryDto> GetCountryByIdAsync(Guid id);
        Task<CountryDto> GetCountryWithStatesAsync(Guid id);
        Task<CountryDto> CreateCountryAsync(CreateCountryDto createCountryDto);
        Task UpdateCountryAsync(Guid id, UpdateCountryDto updateCountryDto);
        Task DeleteCountryAsync(Guid id);
    }
}