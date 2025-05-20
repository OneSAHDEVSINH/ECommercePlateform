using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using ECommercePlatform.Domain.Entities;
using MediatR;


namespace ECommercePlatform.Application.Features.Countries.Commands.Update;

public class UpdateCountryHandler : IRequestHandler<UpdateCountryCommand, AppResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCountryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    public async Task<AppResult> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var country = await _unitOfWork.Countries.GetByIdAsync(request.Id);
            if (country == null)
            {
                return AppResult.Failure($"Country with this ID \"{request.Id}\" not found.");
            }

            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return AppResult.Failure("Country code cannot be null or empty.");
            }

            var isCodeUnique = await _unitOfWork.Countries.IsCodeUniqueAsync(request.Code, request.Id);
            if (!isCodeUnique)
            {
                return AppResult.Failure($"Country with this code \"{request.Code}\" already exists.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return AppResult.Failure("Country name cannot be null or empty.");
            }

            var isNameUnique = await _unitOfWork.Countries.IsNameUniqueAsync(request.Name, request.Id);
            if (!isNameUnique)
            {
                return AppResult.Failure($"Country with this name \"{request.Name}\" already exists.");
            }

            var updatedCountry = (UpdateCountryDto)request;

            country.Update(request.Name, request.Code);

            // Map the updated properties
            //_mapper.Map(request, country);

            // Set the updater information
            //if (_currentUserService.IsAuthenticated)
            //{
            //    //country.ModifiedBy = _currentUserService.UserId;
            //    country.ModifiedBy = request.ModifiedBy;
            //    country.ModifiedOn = DateTime.Now;
            //}
            await _unitOfWork.Countries.UpdateAsync(country);
            //await _unitOfWork.CompleteAsync();
            return AppResult.Success();
        }
        catch (Exception ex)
        {
            return AppResult.Failure($"An error occurred: {ex.Message}");
        }
    }
}
