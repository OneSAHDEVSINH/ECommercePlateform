namespace ECommercePlateform.Application.Interfaces.IAuth
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
    }
}
