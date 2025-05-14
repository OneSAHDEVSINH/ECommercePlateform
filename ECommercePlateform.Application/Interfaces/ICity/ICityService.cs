using ECommercePlateform.Application.DTOs;

namespace ECommercePlateform.Application.Interfaces.ICity
{
    public interface ICityService
    {
        Task<IReadOnlyList<CityDto>> GetAllCitiesAsync();
        Task<CityDto> GetCityByIdAsync(Guid id);
        Task<CityDto> GetCityWithStateAsync(Guid id);
        Task<CityDto> CreateCityAsync(CreateCityDto createCityDto);
        Task UpdateCityAsync(Guid id, UpdateCityDto updateCityDto);
        Task DeleteCityAsync(Guid id);
        Task<IReadOnlyList<CityDto>> GetCitiesByStateIdAsync(Guid stateId);
        Task<IReadOnlyList<CityDto>> GetCitiesByCountryIdAsync(Guid countryId);
    }
}
