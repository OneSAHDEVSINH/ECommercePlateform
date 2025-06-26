using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;

namespace ECommercePlatform.Application.Interfaces.IRepositories
{
    public interface IModuleRepository : IGenericRepository<Module>
    {
        new Task<Module?> GetByIdAsync(Guid id);
        new Task<List<Module>> GetAllAsync();
        Task<List<Module>> GetActiveModulesAsync();
        Task<Module?> GetByRouteAsync(string route);
        Task<Result<string>> EnsureNameIsUniqueAsync(string name, Guid? excludeId = null);
        Task<Result<string>> EnsureRouteIsUniqueAsync(string route, Guid? excludeId = null);
        Task<Result<string>> EnsureIconIsUniqueAsync(string icon, Guid? excludeId = null);

        // Pagination methods
        Task<PagedResponse<Module>> GetPagedModulesAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<ModuleDto>> GetPagedModuleDtosAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<ModuleListDto>> GetPagedModuleListDtosAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);
    }
}