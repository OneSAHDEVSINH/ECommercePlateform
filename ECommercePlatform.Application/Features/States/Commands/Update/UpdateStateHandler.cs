using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IState;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Commands.Update
{
    public class UpdateStateHandler(IStateRepository stateRepository, IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : IRequestHandler<UpdateStateCommand, AppResult>
    {

        private readonly IStateRepository _stateRepository = stateRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        //private readonly IValidator<UpdateStateCommand> _validator;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<AppResult> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate the request object
                if (string.IsNullOrWhiteSpace(request.Code))
                {
                    return AppResult.Failure("State code cannot be null or empty.");
                }

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return AppResult.Failure("State name cannot be null or empty.");
                }

                var state = await _unitOfWork.States.GetByIdAsync(request.Id);
                if (state == null)
                {
                    return AppResult.Failure($"State with this ID \"{request.Id}\" not found.");
                }

                //var isCodeUnique = await _unitOfWork.States.IsCodeUniqueInCountryAsync(request.Code, request.Id);
                //if (!isCodeUnique)
                //{
                //    return AppResult.Failure($"State with this code \"{request.Code}\" already exists.");
                //}

                //var isNameUnique = await _unitOfWork.States.IsNameUniqueInCountryAsync(request.Name, request.Id);
                //if (!isNameUnique)
                //{
                //    return AppResult.Failure($"State with this name \"{request.Name}\" already exists.");
                //}

                var isNameUniqueInCountry = await _unitOfWork.States.EnsureNameIsUniqueInCountryAsync(request.Name, request.CountryId);
                if (isNameUniqueInCountry == null || !isNameUniqueInCountry.IsSuccess)
                {
                    return AppResult.Failure($"State with this name \"{request.Name}\" already exists in this country.");
                }

                var isCodeUniqueInCountry = await _unitOfWork.States.EnsureCodeIsUniqueInCountryAsync(request.Code, request.CountryId);
                if (isNameUniqueInCountry == null || !isNameUniqueInCountry.IsSuccess)
                {
                    return AppResult.Failure($"State with this code \"{request.Code}\" already exists in this country.");
                }
                //_mapper.Map(request, state);

                var updatedState = (UpdateStateDto)request;
                state.Update(request.Name, request.Code, request.CountryId);

                await _unitOfWork.States.UpdateAsync(state);
                //await _unitOfWork.CompleteAsync();
                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
