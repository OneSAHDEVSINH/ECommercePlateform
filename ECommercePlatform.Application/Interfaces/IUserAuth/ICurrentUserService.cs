namespace ECommercePlatform.Application.Interfaces.IUserAuth
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
    }
}
