using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
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
                return await Result.Success(request)
                    .Bind(async req =>
                    {
                        var state = await _unitOfWork.States.GetByIdAsync(req.Id);
                        return state == null
                            ? Result.Failure<State>($"State with ID \"{req.Id}\" not found.")
                            : Result.Success(state);
                    })
                    .Bind(async state =>
                    {
                        var validationResult = await _unitOfWork.States.EnsureNameAndCodeAreUniqueInCountryAsync(
                        request.Name ?? string.Empty,
                        request.Code ?? string.Empty,
                        request.CountryId,
                        request.Id);

                        return validationResult.IsSuccess
                            ? Result.Success(state)
                            : Result.Failure<State>(validationResult.Error);
                    })
                    .Tap(async state =>
                    {
                        state.Update(
                            request.Name ?? string.Empty,
                            request.Code ?? string.Empty,
                            request.CountryId
                        );
                        state.IsActive = request.IsActive;

                        await _unitOfWork.States.UpdateAsync(state);
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