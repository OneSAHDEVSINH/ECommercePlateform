using ECommercePlatform.Server.Core.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommercePlatform.Server.Core.Application.Interfaces
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