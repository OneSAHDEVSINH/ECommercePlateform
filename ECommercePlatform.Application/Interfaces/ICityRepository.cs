﻿using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;

namespace ECommercePlatform.Application.Interfaces
{
    public interface ICityRepository : IGenericRepository<City>
    {
        Task<IReadOnlyList<City>> GetCitiesByStateIdAsync(Guid stateId);
        Task<Result<string>> EnsureNameIsUniqueInStateAsync(string name, Guid stateId, Guid? excludeId = null);

        // New pagination methods
        Task<PagedResponse<City>> GetPagedCitiesAsync(
            PagedRequest request,
            Guid? stateId = null,
            Guid? countryId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<CityDto>> GetPagedCityDtosAsync(
            PagedRequest request,
            Guid? stateId = null,
            Guid? countryId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);
    }
}
