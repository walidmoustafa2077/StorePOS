using System.Security.Claims;
using StorePOS.Domain.Enums;

namespace StorePOS.Api.Authorization
{
    public static class ClaimsHelper
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        public static UserRole GetRole(this ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
            return Enum.TryParse<UserRole>(roleClaim, out var role) ? role : UserRole.Cashier;
        }

        public static bool IsActive(this ClaimsPrincipal user)
        {
            var isActiveClaim = user.FindFirst("IsActive")?.Value;
            return bool.TryParse(isActiveClaim, out var isActive) && isActive;
        }

        public static string? GetIpAddress(this HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
                return context.Request.Headers["X-Forwarded-For"];

            return context.Connection.RemoteIpAddress?.ToString();
        }
    }
}
