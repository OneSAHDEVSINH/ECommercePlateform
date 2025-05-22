using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using MediatR;


namespace ECommercePlatform.Application.Features.Countries.Commands.Update;

public class UpdateCountryHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateCountryCommand, AppResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<AppResult> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var country = await _unitOfWork.Countries.GetByIdAsync(request.Id);
            if (country == null)
            {
                return AppResult.Failure($"Country with this ID \"{request.Id}\" not found.");
            }

            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return AppResult.Failure("Country code cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return AppResult.Failure("Country name cannot be null or empty.");
            }

            var validationResult = await _unitOfWork.Countries.EnsureNameAndCodeAreUniqueAsync(request.Name, request.Code);

            if (validationResult.IsFailure)
                return AppResult.Failure(validationResult.Error);

            var updatedCountry = (UpdateCountryDto)request;
            country.Update(request.Name, request.Code);

            await _unitOfWork.Countries.UpdateAsync(country);
            return AppResult.Success();
        }
        catch (Exception ex)
        {
            return AppResult.Failure($"An error occurred: {ex.Message}");
        }
    }
}
