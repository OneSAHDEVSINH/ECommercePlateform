using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Queries.GetStatesByCountry
{
    public class GetStatesByCountryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetStatesByCountryQuery, AppResult<List<StateDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<StateDto>>> Handle(GetStatesByCountryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await Result.Success(request)
                    .Bind(async req =>
                    {
                        var country = await _unitOfWork.Countries.GetByIdAsync(req.CountryId);
                        return country == null
                            ? Result.Failure<Country>($"Country with this ID \"{req.CountryId}\" not found.")
                            : Result.Success(country);
                    })
                    .Bind(async country =>
                    {
                        var states = await _unitOfWork.States.GetStatesByCountryIdAsync(country.Id);
                        return Result.Success(states);
                    })
                    .Map(states => states.Select(state => (StateDto)state).ToList())
                    .Map(stateDtos => AppResult<List<StateDto>>.Success(stateDtos))
                    .Match(
                        success => success,
                        failure => AppResult<List<StateDto>>.Failure(failure)
                    );
            }
            catch (Exception ex)
            {
                return AppResult<List<StateDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
