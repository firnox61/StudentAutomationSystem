using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentAutomation.Blazor.Auth
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    namespace StudentAutomation.Blazor.Auth
    {
        public class JwtCookieAuthService : IJwtCookieAuthService
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IConfiguration _cfg;
            private const string ApiTokenCookieName = "sa.api_token";

            public JwtCookieAuthService(IHttpContextAccessor httpContextAccessor, IConfiguration cfg)
            {
                _httpContextAccessor = httpContextAccessor;
                _cfg = cfg;
            }

            public async Task SignInWithJwtAsync(string jwt, DateTime? expiresUtc = null)
            {
                var httpContext = _httpContextAccessor.HttpContext
                    ?? throw new InvalidOperationException("No HttpContext");

                var principal = ValidateAndReadPrincipal(jwt);

                var authProps = new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = expiresUtc
                };

                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProps);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = false, // prod'da true
                    Expires = expiresUtc ?? DateTimeOffset.UtcNow.AddHours(2)
                };

                httpContext.Response.Cookies.Append(ApiTokenCookieName, jwt, cookieOptions);
            }

            public async Task SignOutAsync()
            {
                var httpContext = _httpContextAccessor.HttpContext
                    ?? throw new InvalidOperationException("No HttpContext");

                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (httpContext.Request.Cookies.ContainsKey(ApiTokenCookieName))
                {
                    httpContext.Response.Cookies.Append(ApiTokenCookieName, "",
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(-1),
                            HttpOnly = true,
                            SameSite = SameSiteMode.Lax,
                            Secure = false
                        });
                }
            }

            public ClaimsPrincipal? GetCurrentPrincipalFromCookie()
            {
                var httpContext = _httpContextAccessor.HttpContext;
                return httpContext?.User;
            }

            public string? GetRawTokenFromCookie()
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext is null) return null;
                return httpContext.Request.Cookies.TryGetValue(ApiTokenCookieName, out var jwt) ? jwt : null;
            }

            private ClaimsPrincipal ValidateAndReadPrincipal(string jwt)
            {
                var handler = new JwtSecurityTokenHandler();
                var issuer = _cfg["TokenOptions:Issuer"];
                var audience = _cfg["TokenOptions:Audience"];
                var key = _cfg["TokenOptions:SecurityKey"]
                          ?? throw new InvalidOperationException("Missing TokenOptions:SecurityKey");

                var parameters = new TokenValidationParameters
                {
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateIssuer = !string.IsNullOrWhiteSpace(issuer),
                    ValidateAudience = !string.IsNullOrWhiteSpace(audience),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier
                };

                return handler.ValidateToken(jwt, parameters, out _);
            }
        }
    }
}
