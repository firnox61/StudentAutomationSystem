using StudentAutomation.Application.Abstraction;
using System.Security.Claims;

namespace StudentAutomation.WebAPI.getClaim
{


    public sealed class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _http;
        public CurrentUser(IHttpContextAccessor http) => _http = http;

        public int? UserId
        {
            get
            {
                var user = _http.HttpContext?.User;
                var id = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(id, out var v) ? v : null;
            }
        }

        public string? Email =>
            _http.HttpContext?.User?.FindFirst("email")?.Value;

        public IReadOnlyCollection<string> Roles =>
            _http.HttpContext?.User?
                .FindAll(ClaimTypes.Role)?
                .Select(c => c.Value)
                .ToArray()
            ?? Array.Empty<string>();

        public bool IsInRole(string role) =>
            Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}

