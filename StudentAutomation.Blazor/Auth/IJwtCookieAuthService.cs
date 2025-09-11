using System.Security.Claims;

namespace StudentAutomation.Blazor.Auth
{
    public interface IJwtCookieAuthService
    {
        Task SignInWithJwtAsync(string jwt, DateTime? expiresUtc = null);
        Task SignOutAsync();
        ClaimsPrincipal? GetCurrentPrincipalFromCookie();
        string? GetRawTokenFromCookie();
    }
}
