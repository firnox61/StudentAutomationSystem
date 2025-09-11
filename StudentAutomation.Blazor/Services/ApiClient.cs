using StudentAutomation.Blazor.Models;
using StudentAutomation.Blazor.Models.Auth;
using System.Net;

namespace StudentAutomation.Blazor.Services
{
    public sealed class ApiClient : IApiClient
    {
        private readonly HttpClient _http;
        public ApiClient(HttpClient http) => _http = http;

        public async Task<IReadOnlyList<StudentListDto>> GetStudentsAsync()
        {
            // BE: GET /api/students  -> 200 OK -> List<StudentListDto>
            var resp = await _http.GetAsync("students");
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new HttpRequestException($"GET /students failed ({(int)resp.StatusCode}): {msg}");
            }

            var data = await resp.Content.ReadFromJsonAsync<List<StudentListDto>>();
            return data ?? [];
        }

        public async Task<StudentDetailDto?> GetStudentAsync(int id)
        {
            var resp = await _http.GetAsync($"Students/{id}"); // büyük S
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new HttpRequestException($"GET /Students/{id} failed ({(int)resp.StatusCode}): {msg}");
            }

            return await resp.Content.ReadFromJsonAsync<StudentDetailDto>();
        }
        // ---------------- Teachers ----------------
        public async Task<IReadOnlyList<TeacherListDto>> GetTeachersAsync()
        {
            var resp = await _http.GetAsync("Teachers");
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET /Teachers failed ({(int)resp.StatusCode}): {await resp.Content.ReadAsStringAsync()}");

            return await resp.Content.ReadFromJsonAsync<List<TeacherListDto>>() ?? [];
        }

        public async Task<TeacherDetailDto?> GetTeacherAsync(int id)
        {
            var resp = await _http.GetAsync($"Teachers/{id}");
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET /Teachers/{id} failed ({(int)resp.StatusCode}): {await resp.Content.ReadAsStringAsync()}");

            return await resp.Content.ReadFromJsonAsync<TeacherDetailDto>();
        }

        // ---------------- Courses ----------------
        public async Task<IReadOnlyList<CourseListDto>> GetCoursesAsync()
        {
            var resp = await _http.GetAsync("Courses");
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET /Courses failed ({(int)resp.StatusCode}): {await resp.Content.ReadAsStringAsync()}");

            return await resp.Content.ReadFromJsonAsync<List<CourseListDto>>() ?? [];
        }

        public async Task<CourseDetailDto?> GetCourseAsync(int id)
        {
            var resp = await _http.GetAsync($"Courses/{id}");
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET /Courses/{id} failed ({(int)resp.StatusCode}): {await resp.Content.ReadAsStringAsync()}");

            return await resp.Content.ReadFromJsonAsync<CourseDetailDto>();
        }
        public async Task<int> GetStudentCountAsync()
        {
            var resp = await _http.GetAsync("Students");
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET /Students failed ({(int)resp.StatusCode}): {await resp.Content.ReadAsStringAsync()}");

            var list = await resp.Content.ReadFromJsonAsync<List<StudentListDto>>();
            return list?.Count ?? 0;
        }

        public async Task<int> GetTeacherCountAsync()
        {
            var resp = await _http.GetAsync("Teachers");
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET /Teachers failed ({(int)resp.StatusCode}): {await resp.Content.ReadAsStringAsync()}");

            var list = await resp.Content.ReadFromJsonAsync<List<TeacherListDto>>();
            return list?.Count ?? 0;
        }

        public async Task<int> GetCourseCountAsync()
        {
            var resp = await _http.GetAsync("Courses");
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET /Courses failed ({(int)resp.StatusCode}): {await resp.Content.ReadAsStringAsync()}");

            var list = await resp.Content.ReadFromJsonAsync<List<CourseListDto>>();
            return list?.Count ?? 0;
        }
        public async Task<AccessTokenDto> LoginAsync(string email, string password, CancellationToken ct = default)
        {
            var body = new
            {
                Email = email,
                Password = password
            };

            // Backend’de AuthController’da /api/auth/login gibi varsayıyorum.
            using var resp = await _http.PostAsJsonAsync("auth/login", body, ct);
            if (!resp.IsSuccessStatusCode)
            {
                // 401/400 mesajını oku (kullanıcıya göstereceğiz)
                var msg = await resp.Content.ReadAsStringAsync(ct);
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "Giriş başarısız." : msg);
            }

            var dto = await resp.Content.ReadFromJsonAsync<AccessTokenDto>(cancellationToken: ct);
            if (dto is null || string.IsNullOrWhiteSpace(dto.Token))
                throw new InvalidOperationException("Sunucudan geçerli token alınamadı.");

            return dto;
        }
        public async Task<bool> RegisterAsync(RegisterRequest req, CancellationToken ct = default)
        {
            var body = new
            {
                FirstName = req.FirstName,
                LastName = req.LastName,
                Email = req.Email,
                Password = req.Password
            };

            using var resp = await _http.PostAsJsonAsync("auth/register", body, ct);
            if (resp.StatusCode == HttpStatusCode.Conflict) // e-posta zaten var vb.
            {
                var msg = await resp.Content.ReadAsStringAsync(ct);
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "Kullanıcı zaten kayıtlı." : msg);
            }
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync(ct);
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(msg) ? "Kayıt başarısız." : msg);
            }
            return true; // backend User döndürse bile burada başarıyı bool yeterli kabul ediyoruz
        }
    }
}
