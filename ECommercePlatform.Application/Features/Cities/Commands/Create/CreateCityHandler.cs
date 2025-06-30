using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Create
{
    public class CreateCityHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateCityCommand, AppResult<CityDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _unitOfWork.Cities
                    .EnsureNameIsUniqueInStateAsync(request.Name, request.StateId)
                    .Bind(async tuple =>
                    {
                        var city = City.Create(request.Name, request.StateId);
                        city.IsActive = true;
                        await _unitOfWork.Cities.AddAsync(city);
                        return Result.Success(city);
                    })
                    .Map(city => AppResult<CityDto>.Success((CityDto)city))
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