using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Create
{
    public class CreateCityHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateCityCommand, AppResult<CityDto>>
    {

        public async Task<AppResult<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        {
            var isNameUnique = await _unitOfWork.Cities.IsNameUniqueInStateAsync(request.Name, request.StateId);
            if (!isNameUnique)
            {
                return AppResult<CityDto>.Failure($"City with this name \"{request.Name}\" already exists.");
            }

            var city = new City
            {
                Name = request.Name,
                StateId = request.StateId,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = request.CreatedBy,
                IsActive = true
            };

            await _unitOfWork.Cities.AddAsync(city);
            //await _unitOfWork.CompleteAsync();

            var cityDto = _mapper.Map<CityDto>(city);
            return AppResult<CityDto>.Success(cityDto);
        }
    }
}
