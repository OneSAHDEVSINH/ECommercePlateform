using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Commands.Update
{
    public class UpdateStateHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateStateCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Code))
                    return AppResult.Failure("State code cannot be null or empty.");

                if (string.IsNullOrWhiteSpace(request.Name))
                    return AppResult.Failure("State name cannot be null or empty.");

                var state = await _unitOfWork.States.GetByIdAsync(request.Id);
                if (state == null)
                    return AppResult.Failure($"State with this ID \"{request.Id}\" not found.");

                var validationResult = await _unitOfWork.States
                    .EnsureNameAndCodeAreUniqueInCountryAsync(request.Name, request.Code, request.CountryId, request.Id);

                if (validationResult.IsFailure)
                    return AppResult.Failure(validationResult.Error);

                var updatedState = (UpdateStateDto)request;
                state.Update(request.Name, request.Code, request.CountryId);

                await _unitOfWork.States.UpdateAsync(state);
                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
