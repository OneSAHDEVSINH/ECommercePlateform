using AutoMapper;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Features.Countries.Commands.Handlers;

public class CreateCountryHandler : IRequestHandler<CreateCountryCommand, AppResult<CountryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCountryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AppResult<CountryDto>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var isCodeUnique = await _unitOfWork.Countries.IsCodeUniqueAsync(request.Code);
            if (!isCodeUnique)
            {
                return AppResult<CountryDto>.Failure($"Country with this code \"{request.Code}\" already exists.");
            }

            var isNameUnique = await _unitOfWork.Countries.IsNameUniqueAsync(request.Name);
            if (!isNameUnique)
            {
                return AppResult<CountryDto>.Failure($"Country with this name \"{request.Name}\" already exists.");
            }

            var country = new Country
            {
                Name = request.Name,
                Code = request.Code,
                CreatedOn = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Countries.AddAsync(country);
            await _unitOfWork.CompleteAsync();

            var countryDto = _mapper.Map<CountryDto>(country);
            return AppResult<CountryDto>.Success(countryDto);
        }
        catch (Exception ex)
        {
            return AppResult<CountryDto>.Failure($"An error occurred: {ex.Message}");
        }
    }
}
