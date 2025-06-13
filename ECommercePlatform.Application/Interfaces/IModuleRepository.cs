using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces
{
    public interface IModuleRepository : IGenericRepository<Module>
    {
        Task<bool> IsNameUniqueAsync(string name);
        Task<bool> IsNameUniqueAsync(string name, Guid excludeId);
        Task<bool> IsRouteUniqueAsync(string route);
        Task<bool> IsRouteUniqueAsync(string route, Guid excludeId);
        new Task<Module?> GetByIdAsync(Guid id);
        new Task<List<Module>> GetAllAsync();
        Task<List<Module>> GetActiveModulesAsync();
        Task<Module?> GetByRouteAsync(string route);
        Task<bool> AnyAsync(Expression<Func<Module, bool>> predicate);
        IQueryable<Module> AsQueryable();
        Task<Result<string>> EnsureNameIsUniqueAsync(string name, Guid? excludeId = null);
        Task<Result<string>> EnsureRouteIsUniqueAsync(string route, Guid? excludeId = null);

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