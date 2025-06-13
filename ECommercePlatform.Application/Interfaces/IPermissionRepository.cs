using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        Task<Permission?> GetByModuleAndTypeAsync(Guid moduleId, string permissionType);
        Task<List<Permission>> GetByModuleIdAsync(Guid moduleId);
        new Task<List<Permission>> GetAllAsync();
        new Task<Permission?> GetByIdAsync(Guid id);
        Task<List<Permission>> GetActivePermissionsAsync();
        Task<bool> AnyAsync(Expression<Func<Permission, bool>> predicate);
        IQueryable<Permission> AsQueryable();

        // Pagination methods
        Task<PagedResponse<Permission>> GetPagedPermissionsAsync(
            PagedRequest request,
            Guid? moduleId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<PermissionDto>> GetPagedPermissionDtosAsync(
            PagedRequest request,
            Guid? moduleId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<PermissionListDto>> GetPagedPermissionListDtosAsync(
            PagedRequest request,
            Guid? moduleId = null,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);
    }
}




//using CSharpFunctionalExtensions;
//using ECommercePlatform.Application.DTOs;
//using ECommercePlatform.Application.Interfaces.IGeneral;
//using ECommercePlatform.Application.Models;
//using ECommercePlatform.Domain.Entities;
//using System.Linq.Expressions;

//namespace ECommercePlatform.Application.Interfaces
//{
//    public interface IPermissionRepository : IGenericRepository<Permission>
//    {
//        Task<Permission?> GetByModuleAndTypeAsync(Guid moduleId, string permissionType);
//        Task<List<Permission>> GetByModuleIdAsync(Guid moduleId);
//        new Task<List<Permission>> GetAllAsync();
//        new Task<Permission?> GetByIdAsync(Guid id);
//        Task<bool> IsNameUniqueInModuleAsync(string name, Guid moduleId);
//        Task<bool> IsNameUniqueInModuleAsync(string name, Guid moduleId, Guid excludeId);
//        Task<List<Permission>> GetActivePermissionsAsync();
//        Task<bool> AnyAsync(Expression<Func<Permission, bool>> predicate);
//        IQueryable<Permission> AsQueryable();
//        Task<Result<(string name, Guid moduleId)>> EnsureNameIsUniqueInModuleAsync(string name, Guid moduleId, Guid? excludeId = null);

//        // Pagination methods
//        Task<PagedResponse<Permission>> GetPagedPermissionsAsync(
//            PagedRequest request,
//            Guid? moduleId = null,
//            bool activeOnly = true,
//            CancellationToken cancellationToken = default);

//        Task<PagedResponse<PermissionDto>> GetPagedPermissionDtosAsync(
//            PagedRequest request,
//            Guid? moduleId = null,
//            bool activeOnly = true,
//            CancellationToken cancellationToken = default);

//        Task<PagedResponse<PermissionListDto>> GetPagedPermissionListDtosAsync(
//            PagedRequest request,
//            Guid? moduleId = null,
//            bool activeOnly = true,
//            CancellationToken cancellationToken = default);
//    }
//}