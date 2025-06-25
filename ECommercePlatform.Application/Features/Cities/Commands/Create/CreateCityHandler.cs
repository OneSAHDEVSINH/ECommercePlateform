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
                var result = await _unitOfWork.Cities.EnsureNameIsUniqueInStateAsync(request.Name, request.StateId)
                    .Map(_ => // Use _ to ignore the normalized name
                    {
                        // Use the original name from the request
                        var city = City.Create(request.Name, request.StateId);
                        city.IsActive = true;
                        return city;
                    })
                    .Tap(city => _unitOfWork.Cities.AddAsync(city))
                    .Map(city => AppResult<CityDto>.Success((CityDto)city));

                return result.IsSuccess
                    ? result.Value
                    : AppResult<CityDto>.Failure(result.Error);
            }
            catch (Exception ex)
            {
                return AppResult<CityDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}