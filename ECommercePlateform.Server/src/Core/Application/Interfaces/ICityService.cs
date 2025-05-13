using ECommercePlateform.Server.Core.Application.DTOs;

namespace ECommercePlateform.Server.src.Core.Application.Interfaces
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
        //Task<bool> IsCityNameUniqueAsync(string name);
        //Task<bool> IsCityNameUniqueAsync(string name, Guid id);
        //Task<bool> IsCityExistsAsync(Guid id);
        //Task<bool> IsCityExistsByNameAsync(string name);
        //Task<bool> IsCityExistsByNameAsync(string name, Guid id);
        //Task<bool> IsCityExistsByStateIdAsync(Guid stateId);
        //Task<bool> IsCityExistsByStateIdAsync(Guid stateId, Guid id);
        //Task<bool> IsCityExistsByCountryIdAsync(Guid countryId);
        //Task<bool> IsCityExistsByCountryIdAsync(Guid countryId, Guid id);
        //Task<bool> IsCityExistsByStateIdAndNameAsync(Guid stateId, string name);
        //Task<bool> IsCityExistsByStateIdAndNameAsync(Guid stateId, string name, Guid id);
        //Task<bool> IsCityExistsByStateIdAndCodeAsync(Guid stateId, string code);
        //Task<bool> IsCityExistsByStateIdAndCodeAsync(Guid stateId, string code, Guid id);
        //Task<bool> IsCityExistsByStateIdAndNameAndCodeAsync(Guid stateId, string name, string code);
        //Task<bool> IsCityExistsByStateIdAndNameAndCodeAsync(Guid stateId, string name, string code, Guid id);
        //Task<bool> IsCityExistsByCountryIdAndNameAsync(Guid countryId, string name);
        //Task<bool> IsCityExistsByCountryIdAndNameAsync(Guid countryId, string name, Guid id);
        //Task<bool> IsCityExistsByCountryIdAndCodeAsync(Guid countryId, string code);
        //Task<bool> IsCityExistsByCountryIdAndCodeAsync(Guid countryId, string code, Guid id);
        //Task<bool> IsCityExistsByCountryIdAndNameAndCodeAsync(Guid countryId, string name, string code);
        //Task<bool> IsCityExistsByCountryIdAndNameAndCodeAsync(Guid countryId, string name, string code, Guid id);
        //Task<bool> IsCityExistsByStateIdAndCountryIdAsync(Guid stateId, Guid countryId);
        //Task<bool> IsCityExistsByStateIdAndCountryIdAsync(Guid stateId, Guid countryId, Guid id);
        //Task<bool> IsCityExistsByStateIdAndCountryIdAndNameAsync(Guid stateId, Guid countryId, string name);
        //Task<bool> IsCityExistsByStateIdAndCountryIdAndNameAsync(Guid stateId, Guid countryId, string name, Guid id);
        //Task<bool> IsCityExistsByStateIdAndCountryIdAndCodeAsync(Guid stateId, Guid countryId, string code);
        //Task<bool> IsCityExistsByStateIdAndCountryIdAndCodeAsync(Guid stateId, Guid countryId, string code, Guid id);
        //Task<bool> IsCityExistsByStateIdAndCountryIdAndNameAndCodeAsync(Guid stateId, Guid countryId, string name, string code);
        //Task<bool> IsCityExistsByStateIdAndCountryIdAndNameAndCodeAsync(Guid stateId, Guid countryId, string name, string code, Guid id);
        //Task<bool> IsCityExistsByStateIdAndCountryIdAndNameAndCodeAndIsActiveAsync(Guid stateId, Guid countryId, string name, string code, bool isActive);
        //Task<bool> IsCityExistsByStateIdAndCountryIdAndNameAndCodeAndIsActiveAsync(Guid stateId, Guid countryId, string name, string code, bool isActive, Guid id);
        //Task<bool> IsCityExistsByStateIdAndCountryIdAndNameAndCodeAndIsDeletedAsync(Guid stateId, Guid countryId, string name, string code, bool isDeleted);
        //Task<bool> IsCityExistsByStateIdAndCountryIdAndNameAndCodeAndIsDeletedAsync(Guid stateId, Guid countryId, string name, string code, bool isDeleted, Guid id);
        //Task<bool> IsCityExistsByStateIdAndCountryIdAndNameAndCodeAndIsActiveAndIsDeletedAsync(Guid stateId, Guid countryId, string name, string code, bool isActive, bool isDeleted);
        //Task<bool> IsCityExistsByStateIdAndCountryIdAndNameAndCodeAndIsActiveAndIsDeletedAsync(Guid stateId, Guid countryId, string name, string code, bool isActive, bool isDeleted, Guid id);
    }
}
