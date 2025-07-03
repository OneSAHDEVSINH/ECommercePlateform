using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetModuleById
{
    public class GetModuleByIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetModuleByIdQuery, AppResult<ModuleDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<ModuleDto>> Handle(GetModuleByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await Result.Success(request)
                    .Bind(async req =>
                    {
                        var module = await _unitOfWork.Modules.GetByIdAsync(req.Id);
                        return module == null
                            ? Result.Failure<Module>($"Module with ID {req.Id} not found.")
                            : Result.Success(module);
                    })
                    .Map(module => (ModuleDto)module)
                    .Map(moduleDto => AppResult<ModuleDto>.Success(moduleDto))
                    .Match(
                        success => success,
                        failure => AppResult<ModuleDto>.Failure(failure)
                    );
            }
            catch (Exception ex)
            {
                return AppResult<ModuleDto>.Failure($"An error occurred while retrieving the module: {ex.Message}");
            }
        }
    }
}


//    var module = await _unitOfWork.Modules.GetByIdAsync(request.Id);
//    if (module == null)
//        return AppResult<ModuleDto>.Failure($"Module with ID {request.Id} not found.");

//    // Map to DTO using the explicit operator
//    var moduleDto = (ModuleDto)module;

//    return AppResult<ModuleDto>.Success(moduleDto);