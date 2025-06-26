using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommercePlatform.API.Hubs
{
    [Authorize]
    public class PermissionHub(ILogger<PermissionHub> logger) : Hub
    {
        private readonly ILogger<PermissionHub> _logger = logger;

        public override async Task OnConnectedAsync()
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
                    _logger.LogInformation($"User {userId} connected to PermissionHub");
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
                    _logger.LogInformation($"User {userId} disconnected from PermissionHub");
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
