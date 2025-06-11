using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces.IGeneral;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;
using System.Linq.Expressions;

namespace ECommercePlatform.Application.Interfaces.IUserAuth
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> FindUserByEmailAsync(string email);
        Task<User?> FindUserByEmailAndPasswordAsync(string email, string password);
        Task<User?> FindUserWithRolesByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email, Guid excludeUserId);
        new Task<User?> GetByIdAsync(Guid id);
        new Task<List<User>> GetAllAsync();
        Task<List<User>> GetUsersByRoleIdAsync(Guid roleId);
        Task<List<User>> GetActiveUsersAsync();
        Task<bool> AnyAsync(Expression<Func<User, bool>> predicate);
        IQueryable<User> AsQueryable();
        Task<Result<string>> EnsureEmailIsUniqueAsync(string email, Guid? excludeId = null);

        // Pagination methods
        Task<PagedResponse<User>> GetPagedUsersAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<UserDto>> GetPagedUserDtosAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<UserListDto>> GetPagedUserListDtosAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);
    }
}