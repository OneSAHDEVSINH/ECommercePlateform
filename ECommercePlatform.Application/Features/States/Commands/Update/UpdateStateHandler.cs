using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IState;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Commands.Update
{
    public class UpdateStateHandler : IRequestHandler<UpdateStateCommand, AppResult>
    {

        private readonly IStateRepository _stateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        //private readonly IValidator<UpdateStateCommand> _validator;
        private readonly ICurrentUserService _currentUserService;
        //public UpdateStateHandler(IStateRepository stateRepository, IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateStateCommand> validator, ICurrentUserService currentUserService)
        public UpdateStateHandler(IStateRepository stateRepository, IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _stateRepository = stateRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            //_validator = validator;
            _currentUserService = currentUserService;
        }
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

                var isCodeUnique = await _unitOfWork.States.IsCodeUniqueInCountryAsync(request.Code, request.Id);
                if (!isCodeUnique)
                {
                    return AppResult.Failure($"State with this code \"{request.Code}\" already exists.");
                }

                var isNameUnique = await _unitOfWork.States.IsNameUniqueInCountryAsync(request.Name, request.Id);
                if (!isNameUnique)
                {
                    return AppResult.Failure($"State with this name \"{request.Name}\" already exists.");
                }

                _mapper.Map(request, state);

                if (_currentUserService.IsAuthenticated)
                {
                    //state.ModifiedBy = _currentUserService.UserId;
                    state.ModifiedBy = request.ModifiedBy;
                    state.ModifiedOn = DateTime.Now;
                }

                await _unitOfWork.States.UpdateAsync(state);
                await _unitOfWork.CompleteAsync();
                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
