using AutoMapper;
using ECommercePlatform.Application.Common.Extensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;
using System.Diagnostics.Metrics;

namespace ECommercePlatform.Application.Features.Cities.Commands.Create
{
    public class CreateCityHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateCityCommand, AppResult<CityDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<AppResult<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //var isNameUnique = await _unitOfWork.Cities.IsNameUniqueInStateAsync(request.Name, request.StateId);
                //if (!isNameUnique)
                //{
                //    return AppResult<CityDto>.Failure($"City with this name \"{request.Name}\" already exists.");
                //}

                //var city = City.Create(request.Name, request.StateId); // Use the static Create method
                //city.IsActive = true;

                //await _unitOfWork.Cities.AddAsync(city);
                ////await _unitOfWork.CompleteAsync();
                ////var cityDto = _mapper.Map<CityDto>(city);
                //return AppResult<CityDto>.Success((CityDto)city);

                return await _unitOfWork.States.EnsureNameIsUniqueAsync(request.Name)
        .BindAsync(_ =>
        {
            var city = City.Create(request.Name, request.StateId);
            city.IsActive = true;
            return _unitOfWork.Cities.AddAsync(city)
                .ContinueWith(_ => AppResult<CityDto>.Success((CityDto)city));
        });
            }
            catch (Exception ex)
            {
                return AppResult<CityDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
