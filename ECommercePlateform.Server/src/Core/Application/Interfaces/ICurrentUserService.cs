namespace ECommercePlateform.Server.src.Core.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
    }
}
