using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.Interfaces;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Delete
{
    public class DeleteCityHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCityCommand, AppResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await Result.Success(request.Id)
                .Bind(async id =>
                {
                    var city = await _unitOfWork.Cities.GetByIdAsync(id);
                    return city == null
                        ? Result.Failure<Domain.Entities.City>($"City with ID {id} not found.")
                        : Result.Success(city);
                })
                .Tap(async city => await _unitOfWork.Cities.DeleteAsync(city))
                .Map(_ => AppResult.Success())
                .Match(
                    success => success,
                    failure => AppResult.Failure(failure)
                );
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}