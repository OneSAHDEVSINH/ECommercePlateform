using ECommercePlateform.Domain.Entities;

namespace ECommercePlateform.Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> FindUserByEmailAndPasswordAsync(string email, string password);
        Task<User> FindUserByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email);
    }
}