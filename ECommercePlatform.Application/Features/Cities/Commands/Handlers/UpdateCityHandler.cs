using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.ICity;
using ECommercePlatform.Application.Interfaces.IState;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using ECommercePlatform.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Handlers
{
    public class UpdateCityHandler : IRequestHandler<UpdateCityCommand, AppResult>
    {
        private readonly ICityRepository _cityRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public UpdateCityHandler(ICityRepository cityRepository, IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _cityRepository = cityRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<AppResult> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate the request object
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return AppResult.Failure("City name cannot be null or empty.");
                }
                var city = await _unitOfWork.Cities.GetByIdAsync(request.Id);
                if (city == null)
                {
                    return AppResult.Failure($"City with this ID \"{request.Id}\" not found.");
                }
                var isNameUnique = await _unitOfWork.Cities.IsNameUniqueInStateAsync(request.Name, request.Id);
                if (!isNameUnique)
                {
                    return AppResult.Failure($"City with this name \"{request.Name}\" already exists.");
                }

                _mapper.Map(request, city);

                if (_currentUserService.IsAuthenticated)
                {
                    city.ModifiedBy = _currentUserService.UserId;
                    city.ModifiedOn = DateTime.Now;
                }

                await _unitOfWork.Cities.UpdateAsync(city);
                await _unitOfWork.CompleteAsync();
                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
