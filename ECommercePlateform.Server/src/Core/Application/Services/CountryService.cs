using AutoMapper;
using ECommercePlateform.Server.Core.Application.DTOs;
using ECommercePlateform.Server.Core.Application.Interfaces;
using ECommercePlateform.Server.Core.Domain.Entities;
using ECommercePlateform.Server.src.Core.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommercePlateform.Server.Core.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public CountryService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<CountryDto> CreateCountryAsync(CreateCountryDto createCountryDto)
        {
            var country = _mapper.Map<Country>(createCountryDto);
            country.CreatedOn = DateTime.Now;
            country.IsActive = true;
            // Set the creator information
            if (_currentUserService.IsAuthenticated)
            {
                // You can use either the email or user ID as the CreatedBy value
                country.CreatedBy = _currentUserService.UserId ?? _currentUserService.Email;
                country.ModifiedBy = country.CreatedBy;
            }
            var result = await _unitOfWork.Countries.AddAsync(country);
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<CountryDto>(result);
        }

        public async Task DeleteCountryAsync(Guid id)
        {
            var country = await _unitOfWork.Countries.GetByIdAsync(id);
            if (country == null)
                throw new KeyNotFoundException($"Country with ID {id} not found");

            await _unitOfWork.Countries.DeleteAsync(country);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IReadOnlyList<CountryDto>> GetAllCountriesAsync()
        {
            var countries = await _unitOfWork.Countries.GetAllAsync();
            return _mapper.Map<IReadOnlyList<CountryDto>>(countries);
        }

        public async Task<CountryDto> GetCountryByIdAsync(Guid id)
        {
            var country = await _unitOfWork.Countries.GetByIdAsync(id);
            if (country == null)
                throw new KeyNotFoundException($"Country with ID {id} not found");

            return _mapper.Map<CountryDto>(country);
        }

        public async Task<CountryDto> GetCountryWithStatesAsync(Guid id)
        {
            var country = await _unitOfWork.Countries.GetCountryWithStatesAsync(id);
            if (country == null)
                throw new KeyNotFoundException($"Country with ID {id} not found");

            return _mapper.Map<CountryDto>(country);
        }

        public async Task UpdateCountryAsync(Guid id, UpdateCountryDto updateCountryDto)
        {
            var country = await _unitOfWork.Countries.GetByIdAsync(id);
            if (country == null)
                throw new KeyNotFoundException($"Country with ID {id} not found");

            _mapper.Map(updateCountryDto, country);
            country.ModifiedOn = DateTime.Now;

            // Set the modifier information
            if (_currentUserService.IsAuthenticated)
            {
                country.ModifiedBy = _currentUserService.UserId ?? _currentUserService.Email;
            }

            await _unitOfWork.Countries.UpdateAsync(country);
            await _unitOfWork.CompleteAsync();
        }
    }
} 