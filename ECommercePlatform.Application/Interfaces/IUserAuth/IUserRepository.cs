using ECommercePlatform.Application.Interfaces.IGeneral;
using ECommercePlatform.Domain.Entities;

namespace ECommercePlatform.Application.Interfaces.IUserAuth
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> FindUserByEmailAndPasswordAsync(string email, string password);
        Task<User> FindUserByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<User> FindUserWithRolesByEmailAsync(string email);
        new Task<List<User>> GetAllAsync();
    }
}