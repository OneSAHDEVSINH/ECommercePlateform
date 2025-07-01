using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Queries.GetCitiesByState
{
    public class GetCitysByStateHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCitiesByStateQuery, AppResult<List<CityDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<CityDto>>> Handle(GetCitiesByStateQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await Result.Success(request)
                    .Bind(async req =>
                    {
                        var state = await _unitOfWork.States.GetByIdAsync(req.StateId);
                        return state == null
                            ? Result.Failure<State>($"State with ID \"{req.StateId}\" not found.")
                            : Result.Success(state);
                    })
                    .Bind(async state =>
                    {
                        var cities = await _unitOfWork.Cities.GetCitiesByStateIdAsync(state.Id);
                        return Result.Success(cities);
                    })
                    .Map(cities => cities.Select(city => (CityDto)city).ToList())
                    .Map(cityDtos => AppResult<List<CityDto>>.Success(cityDtos))
                    .Match(
                        success => success,
                        failure => AppResult<List<CityDto>>.Failure(failure)
                    );
            }
            catch (Exception ex)
            {
                return AppResult<List<CityDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
