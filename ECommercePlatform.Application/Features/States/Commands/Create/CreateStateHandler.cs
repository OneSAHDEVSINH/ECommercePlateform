using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Commands.Create
{
    public class CreateStateHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateStateCommand, AppResult<StateDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<StateDto>> Handle(CreateStateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.States.EnsureNameAndCodeAreUniqueInCountryAsync(request.Name, request.Code, request.CountryId)
                .Map(tuple =>
                {
                    var state = State.Create(request.Name, request.Code, request.CountryId);
                    state.IsActive = true;
                    return state;
                })
                .Tap(state => _unitOfWork.States.AddAsync(state))
                .Map(state => AppResult<StateDto>.Success((StateDto)state));

                return result.IsSuccess
                    ? result.Value
                    : AppResult<StateDto>.Failure(result.Error);
            }
            catch (Exception ex)
            {
                return AppResult<StateDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}