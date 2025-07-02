using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Queries.GetCityById
{
    public class GetCityByIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCityByIdQuery, AppResult<CityDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<CityDto>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await Result.Success(request)
                    .Bind(async req =>
                    {
                        var city = await _unitOfWork.Cities.GetByIdAsync(req.Id);
                        return city == null
                            ? Result.Failure<City>($"City with this ID \"{req.Id}\" not found.")
                            : Result.Success(city);
                    })
                    .Map(city => (CityDto)city)
                    .Map(cityDto => AppResult<CityDto>.Success(cityDto))
                    .Match(
                        success => success,
                        failure => AppResult<CityDto>.Failure(failure)
                    );
            }
            catch (Exception ex)
            {
                return AppResult<CityDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
