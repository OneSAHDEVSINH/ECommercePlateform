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
        
    }
}
