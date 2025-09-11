using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAutomationMVC.Web.Models.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace StudentAutomationMVC.Web.Controllers
{
    [Authorize] // default: tüm aksiyonlar auth ister
    public class AuthController : Controller
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public AuthController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("UstaApi");
        }

        // ---------- REGISTER ----------
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserForRegisterDto dto, string? returnUrl, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var resp = await _http.PostAsJsonAsync("auth/register", dto, ct);

                if (!resp.IsSuccessStatusCode)
                {
                    await MapApiErrorsToModelState(resp, nameof(Register), ct);
                    TempData["Error"] = "Kayıt başarısız.";
                    return View(dto);
                }

                var payload = await resp.Content.ReadFromJsonAsync<AuthResponseDto>(_json, ct);
                if (payload is { Token.Length: > 0 })
                {
                    await SignInWithClaims(payload, rememberMe: true);
                    TempData["Success"] = "Kayıt başarılı, hoş geldiniz!";
                    return SafeLocalRedirect(returnUrl) ?? RedirectToAction("Index", "Home");
                }

                TempData["Error"] = "Kayıt başarılı fakat oturum açılamadı.";
                return RedirectToAction(nameof(Login));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "API bağlantı hatası. Lütfen tekrar deneyin.");
                return View(dto);
            }
        }

        // ---------- LOGIN ----------
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserForLoginDto dto, string? returnUrl, bool rememberMe, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var resp = await _http.PostAsJsonAsync("auth/login", dto, ct);

                if (!resp.IsSuccessStatusCode)
                {
                    await MapApiErrorsToModelState(resp, nameof(Login), ct);
                    TempData["Error"] = "Giriş başarısız. E-posta veya şifre hatalı olabilir.";
                    return View(dto);
                }

                var payload = await resp.Content.ReadFromJsonAsync<AuthResponseDto>(_json, ct);
                if (payload is { Token.Length: > 0 })
                {
                    await SignInWithClaims(payload, rememberMe);
                    TempData["Success"] = "Giriş başarılı.";
                    return SafeLocalRedirect(returnUrl) ?? RedirectToAction("Index", "Home");
                }

                TempData["Error"] = "API’den geçerli yanıt alınamadı.";
                return View(dto);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "API bağlantı hatası. Lütfen tekrar deneyin.");
                return View(dto);
            }
        }

        // ---------- LOGOUT ----------
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction(nameof(Login));
        }

        // ---------- Yardımcılar ----------
        private async Task SignInWithClaims(AuthResponseDto payload, bool rememberMe)
        {
            // JWT'yi decode et
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(payload.Token);

            // Token içindeki tüm claim'leri al
            var claims = jwt.Claims.ToList();

            // BearerTokenHandler için ham token'ı da claim olarak ekle
            claims.Add(new Claim("jwt", payload.Token));

            // (İsteğe bağlı) Name/email yoksa fallback
            if (!claims.Any(c => c.Type == ClaimTypes.Name || c.Type == "name"))
            {
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email")?.Value ?? "";
                if (!string.IsNullOrEmpty(email))
                    claims.Add(new Claim(ClaimTypes.Name, email));
            }

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            // Expiration UTC olmalı
            var expiresUtc = payload.Expiration.Kind == DateTimeKind.Utc
                ? payload.Expiration
                : DateTime.SpecifyKind(payload.Expiration, DateTimeKind.Utc);

            await HttpContext.SignInAsync("Cookies", principal, new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = expiresUtc
            });
        }

        private IActionResult? SafeLocalRedirect(string? returnUrl)
            => !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : null;

        private async Task MapApiErrorsToModelState(HttpResponseMessage response, string context, CancellationToken ct)
        {
            string body = string.Empty;
            try { body = await response.Content.ReadAsStringAsync(ct); } catch { /* ignore */ }

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                // ProblemDetails
                if (root.TryGetProperty("errors", out var errorsElem) && errorsElem.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in errorsElem.EnumerateObject())
                    {
                        foreach (var v in prop.Value.EnumerateArray())
                            ModelState.AddModelError(prop.Name, v.GetString() ?? "Geçersiz değer.");
                    }
                    return;
                }

                if (root.TryGetProperty("title", out var titleElem))
                {
                    ModelState.AddModelError(string.Empty, $"{context}: {titleElem.GetString()}");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(body))
                {
                    ModelState.AddModelError(string.Empty, $"{context}: {body}");
                    return;
                }
            }
            catch
            {
                // body JSON değilse
            }

            ModelState.AddModelError(string.Empty, $"{context}: {(int)response.StatusCode} {response.ReasonPhrase}");
        }
    }

    // API Login/Register response modeli
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; } // Tercihen UTC
    }
}
