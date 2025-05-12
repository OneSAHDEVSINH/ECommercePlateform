using ECommercePlateform.Server.Core.Application.DTOs;

namespace ECommercePlateform.Server.src.Core.Application.Interfaces
{
    public interface IStateService
    {
        Task<IReadOnlyList<StateDto>> GetAllStatesAsync();
        Task<StateDto> GetStateByIdAsync(Guid id);
        Task<StateDto> GetStateWithCitiesAsync(Guid id);
        Task<StateDto> CreateStateAsync(CreateStateDto createStateDto);
        Task UpdateStateAsync(Guid id, UpdateStateDto updateStateDto);
        Task DeleteStateAsync(Guid id);
        Task<IReadOnlyList<StateDto>> GetStatesByCountryIdAsync(Guid countryId);
        //Task<StateDto> GetStateByCodeAsync(string code);
        //Task<StateDto> GetStateByNameAsync(string name);
        //Task<bool> IsStateNameUniqueAsync(string name);
        //Task<bool> IsStateCodeUniqueAsync(string code);
        //Task<bool> IsStateNameUniqueAsync(string name, Guid id);
        //Task<bool> IsStateCodeUniqueAsync(string code, Guid id);
        //Task<bool> IsStateActiveAsync(Guid id);
        //Task<bool> IsStateDeletedAsync(Guid id);
        //Task<bool> IsStateExistsAsync(Guid id);
        //Task<bool> IsStateExistsByNameAsync(string name);
        //Task<bool> IsStateExistsByCodeAsync(string code);
        //Task<bool> IsStateExistsByNameAsync(string name, Guid id);
        //Task<bool> IsStateExistsByCodeAsync(string code, Guid id);
        //Task<bool> IsStateExistsByCountryIdAsync(Guid countryId);
        //Task<bool> IsStateExistsByCountryIdAsync(Guid countryId, Guid id);
        //Task<bool> IsStateExistsByCountryIdAndNameAsync(Guid countryId, string name);
        //Task<bool> IsStateExistsByCountryIdAndNameAsync(Guid countryId, string name, Guid id);
        //Task<bool> IsStateExistsByCountryIdAndCodeAsync(Guid countryId, string code);
        //Task<bool> IsStateExistsByCountryIdAndCodeAsync(Guid countryId, string code, Guid id);
        //Task<bool> IsStateExistsByCountryIdAndNameAndCodeAsync(Guid countryId, string name, string code);
        //Task<bool> IsStateExistsByCountryIdAndNameAndCodeAsync(Guid countryId, string name, string code, Guid id);
    }
}
