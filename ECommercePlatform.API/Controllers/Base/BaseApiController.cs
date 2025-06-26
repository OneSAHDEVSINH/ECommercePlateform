using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommercePlatform.API.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected Guid CurrentUserId
        {
            get
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
            }
        }

        protected string CurrentUserEmail => User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

        protected bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;

        protected bool IsSuperAdmin => User.HasClaim(c => c.Type == "SuperAdmin" && c.Value == "true");

        protected bool HasRole(string roleName) => User.IsInRole(roleName);

        protected bool HasPermission(string module, string permission)
        {
            var permissionClaim = $"{module}:{permission}";
            return User.HasClaim("Permission", permissionClaim);
        }
    }
}