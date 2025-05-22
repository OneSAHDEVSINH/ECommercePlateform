using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Update
{
    public class UpdateCityHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateCityCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate the request object
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return AppResult.Failure("City name cannot be null or empty.");
                }

                var city = await _unitOfWork.Cities.GetByIdAsync(request.Id);
                if (city == null)
                {
                    return AppResult.Failure($"City with this ID \"{request.Id}\" not found.");
                }

                var isNameUniqueInState = await _unitOfWork.Cities.EnsureNameIsUniqueInStateAsync(request.Name, request.StateId);
                if (isNameUniqueInState.IsFailure)
                {
                    return AppResult.Failure(isNameUniqueInState.Error);
                }

                var updatedCity = (UpdateCityDto)request;
                city.Update(request.Name, request.StateId);

                await _unitOfWork.Cities.UpdateAsync(city);
                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
