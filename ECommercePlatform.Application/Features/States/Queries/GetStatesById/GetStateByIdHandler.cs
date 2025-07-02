using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.States.Queries.GetStatesById
{
    public class GetStateByIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetStateByIdQuery, AppResult<StateDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<StateDto>> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await Result.Success(request)
                    .Bind(async req => 
                    {
                        var state = await _unitOfWork.States.GetByIdAsync(req.Id);
                        return state == null
                            ? Result.Failure<State>($"State with this ID \"{req.Id}\" not found.")
                            : Result.Success(state);
                    })
                    .Map(state => (StateDto)state)
                    .Map(stateDto => AppResult<StateDto>.Success(stateDto))
                    .Match(
                        success => success,
                        failure => AppResult<StateDto>.Failure(failure)
                    );
            }
            catch (Exception ex)
            {
                return AppResult<StateDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
