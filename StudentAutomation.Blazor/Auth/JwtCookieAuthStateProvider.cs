using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StudentAutomation.Blazor.Auth
{
    /// <summary>
    /// Blazor'a "kullanıcının kim olduğu" bilgisini verir.
    /// Önce Cookie Auth'taki kimliği okur; yoksa sa.api_token içindeki JWT'den principal üretir.
    /// </summary>
    public class JwtCookieAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string ApiTokenCookieName = "sa.api_token";

        public JwtCookieAuthStateProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var http = _httpContextAccessor.HttpContext;

            if (http?.User?.Identity?.IsAuthenticated == true)
                return Task.FromResult(new AuthenticationState(http.User));

            ClaimsPrincipal principal = new(new ClaimsIdentity());

            if (http is not null &&
                http.Request.Cookies.TryGetValue(ApiTokenCookieName, out var jwt) &&
                !string.IsNullOrWhiteSpace(jwt))
            {
                try
                {
                    var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
                    var identity = new ClaimsIdentity(token.Claims, "jwt");
                    principal = new ClaimsPrincipal(identity);
                }
                catch { }
            }

            return Task.FromResult(new AuthenticationState(principal));
        }

        public void ForceRefresh() =>
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
