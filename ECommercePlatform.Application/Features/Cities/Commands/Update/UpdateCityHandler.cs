using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.ICity;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;

namespace ECommercePlatform.Application.Features.Cities.Commands.Update
{
    public class UpdateCityHandler(ICityRepository cityRepository, IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : IRequestHandler<UpdateCityCommand, AppResult>
    {
        private readonly ICityRepository _cityRepository = cityRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ICurrentUserService _currentUserService = currentUserService;

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
                //var isNameUnique = await _unitOfWork.Cities.IsNameUniqueInStateAsync(request.Name, request.Id);
                //if (!isNameUnique)
                //{
                //    return AppResult.Failure($"City with this name \"{request.Name}\" already exists.");
                //}

                var isNameUniqueInCountry = await _unitOfWork.Cities.EnsureNameIsUniqueInStateAsync(request.Name, request.StateId);
                if (isNameUniqueInCountry == null || !isNameUniqueInCountry.IsSuccess)
                {
                    return AppResult.Failure($"City with this name \"{request.Name}\" already exists in this state.");
                }

                //_mapper.Map(request, city);

                var updatedCity = (UpdateCityDto)request;
                city.Update(request.Name, request.StateId);

                //if (_currentUserService.IsAuthenticated)
                //{
                //    //city.ModifiedBy = _currentUserService.UserId;
                //    city.ModifiedBy = request.ModifiedBy;
                //    city.ModifiedOn = DateTime.Now;
                //}

                await _unitOfWork.Cities.UpdateAsync(city);
                //await _unitOfWork.CompleteAsync();
                return AppResult.Success();
            }
            catch (Exception ex)
            {
                return AppResult.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
