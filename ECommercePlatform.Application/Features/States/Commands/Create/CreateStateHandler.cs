using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;
using System.Diagnostics.Metrics;

namespace ECommercePlatform.Application.Features.States.Commands.Create
{
    public class CreateStateHandler : IRequestHandler<CreateStateCommand, AppResult<StateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateStateHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<AppResult<StateDto>> Handle(CreateStateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var isCodeUnique = await _unitOfWork.States.IsCodeUniqueInCountryAsync(request.Code, request.CountryId);
                if (!isCodeUnique)
                {
                    return AppResult<StateDto>.Failure($"State with this code \"{request.Code}\" already exists.");
                }
                var isNameUnique = await _unitOfWork.States.IsCodeUniqueInCountryAsync(request.Name, request.CountryId);
                if (!isNameUnique)
                {
                    return AppResult<StateDto>.Failure($"State with this name \"{request.Name}\" already exists.");
                }

                var state = State.Create(request.Name, request.Code, request.CountryId); // Use the static Create method
                state.IsActive = true;

                await _unitOfWork.States.AddAsync(state);
                var stateDto = _mapper.Map<StateDto>(state); // Use the mapper to map the entity to DTO
                return AppResult<StateDto>.Success(stateDto);
            }
            catch (Exception ex)
            {
                return AppResult<StateDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
