using ECommercePlatform.Server.Core.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace ECommercePlatform.Server.Core.Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> FindUserByEmailAndPasswordAsync(string email, string password);
        Task<User> FindUserByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email);
    }
} 