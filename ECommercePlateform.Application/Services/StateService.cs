using AutoMapper;
using ECommercePlateform.Application.DTOs;
using ECommercePlateform.Application.Interfaces;
using ECommercePlateform.Domain.Entities;
using ECommercePlateform.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace ECommercePlateform.Application.Services
{
    public class StateService : IStateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public StateService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<StateDto> CreateStateAsync(CreateStateDto createStateDto)
        {
            //check for regular expression
            if (!ValidationService.IsValidNameorCode(createStateDto.Name, createStateDto.Code, out var errorMessage))
            {
                throw new ValidationException(errorMessage);
            }

            // Check for duplicates
            bool isNameUnique = await _unitOfWork.States.IsNameUniqueInCountryAsync(createStateDto.Name, createStateDto.CountryId);
            if (!isNameUnique)
            {
                throw new DuplicateResourceException($"A state with the name '{createStateDto.Name}' already exists in the selected country.");
            }

            bool isCodeUnique = await _unitOfWork.States.IsCodeUniqueInCountryAsync(createStateDto.Code, createStateDto.CountryId);
            if (!isCodeUnique)
            {
                throw new DuplicateResourceException($"A state with the code '{createStateDto.Code}' already exists in the selected country.");
            }

            var state = _mapper.Map<State>(createStateDto);
            state.CreatedOn = DateTime.Now;
            state.IsActive = true;

            // Set the creator information
            if (_currentUserService.IsAuthenticated)
            {
                // You can use either the email or user ID as the CreatedBy value
                state.CreatedBy = _currentUserService.UserId ?? _currentUserService.Email;
                state.ModifiedBy = state.CreatedBy;
            }

            var result = await _unitOfWork.States.AddAsync(state);

            await _unitOfWork.CompleteAsync();
            return _mapper.Map<StateDto>(result);
        }
        public async Task DeleteStateAsync(Guid id)
        {
            var state = await _unitOfWork.States.GetByIdAsync(id);

            if (state == null)
                throw new KeyNotFoundException($"State with ID {id} not found");

            await _unitOfWork.States.DeleteAsync(state);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IReadOnlyList<StateDto>> GetAllStatesAsync()
        {
            var states = await _unitOfWork.States.GetAllAsync();
            return _mapper.Map<IReadOnlyList<StateDto>>(states);
        }

        public async Task<StateDto> GetStateByIdAsync(Guid id)
        {
            var state = await _unitOfWork.States.GetByIdAsync(id);

            if (state == null)
                throw new KeyNotFoundException($"State with ID {id} not found");

            return _mapper.Map<StateDto>(state);
        }

        public async Task<StateDto> GetStateWithCitiesAsync(Guid id)
        {
            var state = await _unitOfWork.States.GetStateWithCitiesAsync(id);

            if (state == null)
                throw new KeyNotFoundException($"State with ID {id} not found");

            //var cities = await _unitOfWork.Cities.GetAllAsync(c => c.StateId == id);
            //state.Cities = cities.ToList();
            return _mapper.Map<StateDto>(state);
        }

        public async Task UpdateStateAsync(Guid id, UpdateStateDto updateStateDto)
        {
            var state = await _unitOfWork.States.GetByIdAsync(id);
            if (state == null)
                throw new KeyNotFoundException($"State with ID {id} not found");

            // Check if Name is null before calling ValidationService
            if (updateStateDto.Name != null && updateStateDto.Code != null &&
                !ValidationService.IsValidNameorCode(updateStateDto.Name, updateStateDto.Code, out var errorMessage))
            {
                throw new ValidationException(errorMessage);
            }

            // Check for duplicates if name or code is being updated
            if (updateStateDto.Name != null && updateStateDto.Name != state.Name)
            {
                bool isNameUnique = await _unitOfWork.States.IsNameUniqueInCountryAsync(updateStateDto.Name, updateStateDto.CountryId, id);
                if (!isNameUnique)
                {
                    throw new DuplicateResourceException($"A state with the name '{updateStateDto.Name}' already exists in the selected country.");
                }

            }

            if (updateStateDto.Code != null && updateStateDto.Code != state.Code)
            {
                bool isCodeUnique = await _unitOfWork.States.IsCodeUniqueInCountryAsync(updateStateDto.Code, updateStateDto.CountryId, id);
                if (!isCodeUnique)
                {
                    throw new DuplicateResourceException($"A state with the code '{updateStateDto.Code}' already exists in the selected country.");
                }
            }

            _mapper.Map(updateStateDto, state);
            state.ModifiedOn = DateTime.Now;

            // Set the modifier information
            if (_currentUserService.IsAuthenticated)
            {
                state.ModifiedBy = _currentUserService.UserId ?? _currentUserService.Email;
            }

            await _unitOfWork.States.UpdateAsync(state);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<StateDto> RestoreStateAsync(Guid id)
        {
            var state = await _unitOfWork.States.GetByIdAsync(id);

            if (state == null)
                throw new KeyNotFoundException($"State with ID {id} not found");

            state.IsDeleted = false;

            await _unitOfWork.CompleteAsync();
            return _mapper.Map<StateDto>(state);
        }

        public async Task<StateDto> SoftDeleteStateAsync(Guid id)
        {
            var state = await _unitOfWork.States.GetByIdAsync(id);

            if (state == null)
                throw new KeyNotFoundException($"State with ID {id} not found");

            state.IsDeleted = true;

            await _unitOfWork.CompleteAsync();
            return _mapper.Map<StateDto>(state);
        }

        public async Task<IReadOnlyList<StateDto>> GetStatesByCountryIdAsync(Guid countryId)
        {
            // First check if country exists
            var country = await _unitOfWork.Countries.GetByIdAsync(countryId);

            if (country == null)
                throw new KeyNotFoundException($"Country with ID {countryId} not found");

            // Get states by country ID using the repository method
            var states = await _unitOfWork.States.GetStatesByCountryIdAsync(countryId);

            // Map the entities to DTOs and return
            return _mapper.Map<IReadOnlyList<StateDto>>(states);
        }

        //public async Task<IReadOnlyList<StateDto>> GetStatesByCountryIdAsync(Guid countryId)
        //{
        //    var states = await _unitOfWork.States.GetAllAsync(s => s.CountryId == countryId);
        //    return _mapper.Map<IReadOnlyList<StateDto>>(states);
        //}

        //public async Task<StateDto> GetStateByNameAsync(string name)
        //{
        //    var state = await _unitOfWork.States.GetByNameAsync(name);
        //    if (state == null)
        //        throw new KeyNotFoundException($"State with name {name} not found");
        //    return _mapper.Map<StateDto>(state);
        //}
    }
}
