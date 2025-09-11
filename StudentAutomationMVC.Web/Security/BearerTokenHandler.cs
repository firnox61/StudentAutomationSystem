using System.Net.Http.Headers;

namespace StudentAutomationMVC.Web.Security
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _http;

        public BearerTokenHandler(IHttpContextAccessor http)
        {
            _http = http;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1) Cookie auth ile SignInAsync sırasında claim’e koyduysak:
            var jwt = _http.HttpContext?.User?.FindFirst("jwt")?.Value;

            // 2) Eski yöntemle düz cookie’ye yazıyorsan (AuthToken), buradan da dene:
            if (string.IsNullOrEmpty(jwt))
            {
                _http.HttpContext?.Request?.Cookies?.TryGetValue("AuthToken", out jwt);
            }

            if (!string.IsNullOrWhiteSpace(jwt))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
                Console.WriteLine($"[BearerTokenHandler] Auth header set. First 16: {jwt[..Math.Min(16, jwt.Length)]}...");
            }
            else
            {
                Console.WriteLine("[BearerTokenHandler] JWT bulunamadı, Authorization eklenmedi!");
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
