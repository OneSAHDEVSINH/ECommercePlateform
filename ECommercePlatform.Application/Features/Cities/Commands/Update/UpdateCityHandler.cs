using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
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
                return await Result.Success(request)
                    .Bind(async req =>
                    {
                        var city = await _unitOfWork.Cities.GetByIdAsync(req.Id);
                        return city == null
                            ? Result.Failure<City>($"City with ID \"{req.Id}\" not found.")
                            : Result.Success(city);
                    })
                    .Bind(async city =>
                    {
                        var validationResult = await _unitOfWork.Cities.EnsureNameIsUniqueInStateAsync(
                        request.Name ?? string.Empty,
                        request.StateId,
                        request.Id);

                        return validationResult.IsSuccess
                            ? Result.Success(city)
                            : Result.Failure<City>(validationResult.Error);
                    })
                    .Tap(async city =>
                    {
                        city.Update(
                            request.Name ?? string.Empty,
                            request.StateId
                        );
                        city.IsActive = request.IsActive;

                        await _unitOfWork.Cities.UpdateAsync(city);
                    })
                    .Map(_ => AppResult.Success())
                    .Match(
                        success => success,
                        error => AppResult.Failure(error)
                    );
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}