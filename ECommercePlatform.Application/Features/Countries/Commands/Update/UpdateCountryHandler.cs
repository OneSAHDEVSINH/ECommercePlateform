using AutoMapper;
using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Extensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Application.Interfaces.IUserAuth;
using MediatR;


namespace ECommercePlatform.Application.Features.Countries.Commands.Update;

public class UpdateCountryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : IRequestHandler<UpdateCountryCommand, AppResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUserService = currentUserService;

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

            await _unitOfWork.Countries.UpdateAsync(country);
            //await _unitOfWork.CompleteAsync();
            return AppResult.Success();


            //            return await _unitOfWork.Countries.GetByIdAsync(request.Id)
            //             .ToResult($"Country with this ID \"{request.Id}\" not found.")
            //             .BindAsync(country =>
            //             ValidateCode(request.Code, request.Id)
            //             .BindAsync(_ => ValidateName(request.Name, request.Id))
            //             .BindAsync(_ =>
            //             {
            //                 country.Update(request.Name, request.Code);

            //                 // Optionally set audit info
            //                 // country.ModifiedBy = request.ModifiedBy;
            //                 // country.ModifiedOn = DateTime.UtcNow;

            //                 return _unitOfWork.Countries.UpdateAsync(country)
            //.ContinueWith(_ => Result.Success());
            //             }))
            //             .Map(_ => AppResult.Success())
            //             .OnFailure(error => AppResult.Failure(error));


            //return await _unitOfWork.Countries.GetByIdAsync(request.Id)
            //    .ToResult($"Country with this ID \"{request.Id}\" not found.")
            //    .Bind(country =>
            //    {
            //        return _unitOfWork.Countries.EnsureCodeIsUniqueAsync(request.Code, request.Id)
            //            .Bind(_ => _unitOfWork.Countries.EnsureNameIsUniqueAsync(request.Name, request.Id))
            //            .Tap(() =>
            //            {
            //                country.Update(request.Name, request.Code);
            //                return _unitOfWork.Countries.UpdateAsync(country);
            //            });
            //    })
            //    .Map(() => AppResult.Success())
            //    .OnFailure(error => AppResult.Failure(error));

            //return await _unitOfWork.Countries.GetByIdAsync(request.Id)
            //    .ToResult($"Country with this ID \"{request.Id}\" not found.")
            //    .BindAsync(_ => _unitOfWork.Countries.EnsureCodeIsUniqueAsync(request.Code, request.Id)
            //    .BindAsync(_ => _unitOfWork.Countries.EnsureNameIsUniqueAsync(request.Name, request.Id))
            //    .BindAsync(_ =>
            //    {
            //        var country = Country.Create(request.Name, request.Code);
            //        country.IsActive = true;
            //        return _unitOfWork.Countries.UpdateAsync(country)
            //            .ContinueWith(_ => AppResult.Success());
            //    });
            //    return await _unitOfWork.Countries.EnsureCodeIsUniqueAsync(request.Code)
            //.BindAsync(_ => _unitOfWork.Countries.EnsureNameIsUniqueAsync(request.Name))
            //.BindAsync(_ =>
            //{
            //    var country = Country.Create(request.Name, request.Code);
            //    country.IsActive = true;
            //    return _unitOfWork.Countries.AddAsync(country)
            //        .ContinueWith(_ => AppResult<CountryDto>.Success((CountryDto)country));
            //});

            //return await _unitOfWork.Countries.GetByIdAsync(request.Id)
            //    .ToResult($"Country with this ID \"{request.Id}\" not found.")
            //    .Bind(countryTask => countryTask // Unwrap the Task<Country> from the Result
            //        .ContinueWith(async countryResult =>
            //        {
            //            var country = await countryResult;
            //            return await _unitOfWork.Countries.EnsureCodeIsUniqueAsync(request.Code!, request.Id)
            //                .BindAsync(_ => _unitOfWork.Countries.EnsureNameIsUniqueAsync(request.Name!, request.Id))
            //                .BindAsync(_ =>
            //                {
            //                    country.Update(request.Name, request.Code);
            //                    return _unitOfWork.Countries.UpdateAsync(country)
            //                        .ContinueWith(_ => Result.Success());
            //                });
            //        }).Unwrap()) // Unwrap the nested Task
            //    .Map(_ => AppResult.Success())
            //    .OnFailure(error => AppResult.Failure(error));


            //            return await _unitOfWork.Countries.GetByIdAsync(request.Id)
            //             .ToResult($"Country with this ID \"{request.Id}\" not found.")
            //             .BindAsync(country =>
            //             _unitOfWork.Countries.EnsureCodeIsUniqueAsync(request.Code, request.Id)
            //             .BindAsync(_ => _unitOfWork.Countries.EnsureNameIsUniqueAsync(request.Name, request.Id))
            //             .BindAsync(_ =>
            //             {
            //                 country.Update(request.Name, request.Code);
            //                 return _unitOfWork.Countries.UpdateAsync(country)
            //.ContinueWith(_ => Result.Success());
            //             }))
            //             .Map(_ => AppResult.Success())
            //             .OnFailure(error => AppResult.Failure(error));

            //return await _unitOfWork.Countries.GetByIdAsync(request.Id)
            //    .ToResult($"Country with this ID \"{request.Id}\" not found.")
            //    .Bind(country =>
            //        _unitOfWork.Countries.EnsureCodeIsUniqueAsync(request.Code!, request.Id)
            //            .BindAsync(_ => _unitOfWork.Countries.EnsureNameIsUniqueAsync(request.Name!, request.Id))
            //            .BindAsync(_ =>
            //            {
            //                var updatedCountry = (UpdateCountryDto)request;
            //                country.Update(request.Name!, request.Code!);
            //                return _unitOfWork.Countries.UpdateAsync(country)
            //                    .ContinueWith(_ => Result.Success());
            //            }))
            //    .Map(_ => AppResult.Success())
            //    .OnFailure(error => AppResult.Failure(error));


        }
        catch (Exception ex)
        {
            return AppResult.Failure($"An error occurred: {ex.Message}");
        }
    }
}
