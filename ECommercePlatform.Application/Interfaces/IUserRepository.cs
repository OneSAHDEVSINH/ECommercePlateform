using CSharpFunctionalExtensions;
using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Models;
using ECommercePlatform.Domain.Entities;

namespace ECommercePlatform.Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> FindUserByEmailAsync(string email);
        new Task<User?> GetByIdAsync(Guid id);
        new Task<List<User>> GetAllAsync();
        Task<List<User>> GetUsersByRoleIdAsync(Guid roleId);
        Task<List<User>> GetActiveUsersAsync();
        Task<Result<string>> EnsureEmailIsUniqueAsync(string email, Guid? excludeId = null);
        Task<Result<string>> EnsurePhoneIsUniqueAsync(string phone, Guid? excludeId = null);

        // Pagination methods
        Task<PagedResponse<User>> GetPagedUsersAsync(
            PagedRequest request,
            bool activeOnly = true,
            bool includeRoles = true,
            Guid? roleId = null,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<UserDto>> GetPagedUserDtosAsync(
            PagedRequest request,
            bool activeOnly = true,
            bool includeRoles = true,
            Guid? roleId = null,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<UserListDto>> GetPagedUserListDtosAsync(
            PagedRequest request,
            bool activeOnly = true,
            CancellationToken cancellationToken = default);
    }
}