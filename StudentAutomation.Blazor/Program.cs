using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using StudentAutomation.Blazor.Auth;
using StudentAutomation.Blazor.Auth.StudentAutomation.Blazor.Auth;
using StudentAutomation.Blazor.Components;
using StudentAutomation.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Services --------------------
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// Blazor tarafında AuthorizeView / [Authorize]
builder.Services.AddCascadingAuthenticationState();

// Cookie tabanlı kimlik doğrulama
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Cookie.Name = "sa.auth";
        o.LoginPath = "/auth/login";
        o.LogoutPath = "/auth/logout";
        o.AccessDeniedPath = "/denied";
        o.SlidingExpiration = true;
        o.ExpireTimeSpan = TimeSpan.FromHours(2);
        o.Cookie.HttpOnly = true;
        o.Cookie.SameSite = SameSiteMode.Lax;
        // Prod:
        // o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

// Basit rol policy’leri (opsiyonel)
builder.Services.AddAuthorization();

// ÖNEMLİ: Blazor’un kimlik durumunu sağlayan provider
builder.Services.AddScoped<IJwtCookieAuthService, JwtCookieAuthService>();
builder.Services.AddScoped<BearerTokenHandler>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtCookieAuthStateProvider>();

builder.Services.AddHttpContextAccessor();

// JWT'yi cookie'den okuyup API isteklerine Bearer ekleyen handler
builder.Services.AddScoped<BearerTokenHandler>();

// JWT’yi üretip/yenileyip cookie’yi yöneten servis
builder.Services.AddScoped<IJwtCookieAuthService, JwtCookieAuthService>();

// API Base URL
const string ApiBase = "http://localhost:5180/api/";

// Typed HttpClient + BearerTokenHandler
/*builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri(ApiBase);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<BearerTokenHandler>();*/
builder.Services.AddHttpClient<IApiClient, ApiClient>(c => { c.BaseAddress = new Uri("http://localhost:5180/api/"); })
                 .AddHttpMessageHandler<BearerTokenHandler>();
// -------------------- Pipeline --------------------
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// Enhanced forms kullanmıyorsan şart değil; kullanıyorsan kalsın
 app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
