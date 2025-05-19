using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Handlers
{
    public class CreateCityHandler : IRequestHandler<CreateCityCommand, AppResult<CityDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateCityHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<AppResult<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        {
            var isNameUnique = await _unitOfWork.Cities.IsNameUniqueInStateAsync(request.Name, request.StateId);
            if (!isNameUnique)
            {
                return AppResult<CityDto>.Failure("City with this name already exists.");
            }

            var city = new City
            {
                Name = request.Name,
                StateId = request.StateId,
                CreatedOn = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Cities.AddAsync(city);
            await _unitOfWork.CompleteAsync();

            var cityDto = _mapper.Map<CityDto>(city);
            return AppResult<CityDto>.Success(cityDto);
        }
    }
}
