namespace ECommercePlatform.Application.Interfaces.IServices
{
    public interface ISuperAdminService
    {
        bool IsSuperAdminEmail(string email);
        Task<bool> IsSuperAdminAsync(Guid userId);
    }
}
