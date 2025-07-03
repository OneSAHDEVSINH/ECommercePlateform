using CSharpFunctionalExtensions;
using ECommercePlatform.Application.Common.Models;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Domain.Entities;
using MediatR;

namespace ECommercePlatform.Application.Features.Modules.Queries.GetModuleByRoute
{
    public class GetModuleByRouteHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetModuleByRouteQuery, AppResult<List<ModuleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AppResult<List<ModuleDto>>> Handle(GetModuleByRouteQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await Result.Success(request)
                    .Bind(async req =>
                    {
                        var module = await _unitOfWork.Modules.GetByRouteAsync(req.Route);
                        return module == null
                            ? Result.Failure<Module>($"Module with route '{req.Route}' not found.")
                            : Result.Success(module);
                    })
                    .Map(module =>
                    {
                        // Map to DTO using the explicit operator
                        return (ModuleDto)module;
                    })
                    .Map(moduleDto => AppResult<List<ModuleDto>>.Success(new List<ModuleDto> { moduleDto }))
                    .Match(
                        success => success,
                        failure => AppResult<List<ModuleDto>>.Failure(failure)
                    );
            }
            catch (Exception ex)
            {
                return AppResult<List<ModuleDto>>.Failure($"An error occurred while retrieving the module: {ex.Message}");
            }
        }
    }
}


//var module = await _unitOfWork.Modules.GetByRouteAsync(request.Route);
//if (module == null)
//    return AppResult<ModuleDto>.Failure($"Module with route '{request.Route}' not found.");

//// Map to DTO using the explicit operator
//var moduleDto = (ModuleDto)module;

//return AppResult<ModuleDto>.Success(moduleDto);