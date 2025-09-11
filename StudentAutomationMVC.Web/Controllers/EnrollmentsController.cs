using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentAutomationMVC.Web.Models.Courses;
using StudentAutomationMVC.Web.Models.Enrollments;
using StudentAutomationMVC.Web.Models.Shared;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudentAutomationMVC.Web.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public EnrollmentsController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("UstaApi"); // BaseAddress = http://localhost:5180/api/
        }

        // -------- INDEX (senin kodun aynı kalsın) --------
        public async Task<IActionResult> Index(int? studentId, string? studentName, CancellationToken ct)
        {
            var students = await FetchStudentsAsync(ct);
            ViewBag.Students = students
                .OrderBy(s => s.FullName)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FullName} ({s.StudentNumber})",
                    Selected = studentId == s.Id
                })
                .ToList();

            ViewBag.StudentId = studentId;

            if ((!studentId.HasValue || studentId <= 0) && !string.IsNullOrWhiteSpace(studentName))
            {
                var matches = students
                    .Where(s => string.Equals(s.FullName, studentName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (matches.Count == 1)
                    studentId = matches[0].Id;
                else if (matches.Count > 1)
                {
                    TempData["Info"] = "Birden fazla öğrenci eşleşti. Lütfen listeden seçiniz.";
                    return View(new List<CourseMiniDto>());
                }
                else
                {
                    TempData["Info"] = "Öğrenci bulunamadı. Lütfen listeden seçiniz.";
                    return View(new List<CourseMiniDto>());
                }
            }

            if (studentId is null || studentId <= 0)
                return View(new List<CourseMiniDto>());

            try
            {
                var list = await _http.GetFromJsonAsync<List<CourseMiniDto>>(
                    $"Enrollments/student/{studentId}/courses", _json, ct)
                    ?? new List<CourseMiniDto>();

                return View(list);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return View(new List<CourseMiniDto>());
            }
        }

        // -------- CREATE (Derse Kaydet) --------
        // GET: /Enrollments/Create?studentId=1
        public async Task<IActionResult> Create(int? studentId, CancellationToken ct)
        {
            // Öğrenci seçimi için dropdown
            var students = await FetchStudentsAsync(ct);
            ViewBag.Students = students
                .OrderBy(s => s.FullName)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FullName} ({s.StudentNumber})",
                    Selected = studentId == s.Id
                })
                .ToList();

            // Ders seçimi için dropdown (öğrenci varsa, zaten kayıtlı olduklarını çıkar)
            List<CourseMiniDto> courses = studentId.HasValue && studentId > 0
                ? await FetchAvailableCoursesForStudentAsync(studentId.Value, ct)
                : await FetchCoursesAsync(ct);

            ViewBag.Courses = courses
                .OrderBy(c => c.Code)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Code} - {c.Name}"
                })
                .ToList();

            var model = new EnrollRequestDto
            {
                StudentId = studentId ?? 0,
                EnrolledAt = DateOnly.FromDateTime(DateTime.Today) // ← varsayılan bugün
            };
            return View(model);
        }

        // POST: /Enrollments/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnrollRequestDto model, CancellationToken ct)
        {
            // Basit validasyon (ID'ler 0 gelmesin)
            if (model.StudentId <= 0)
                ModelState.AddModelError(nameof(model.StudentId), "Öğrenci seçiniz.");
            if (model.CourseId <= 0)
                ModelState.AddModelError(nameof(model.CourseId), "Ders seçiniz.");

            if (!ModelState.IsValid)
            {
                // Dropdown'ları yeniden doldur
                var students = await FetchStudentsAsync(ct);
                ViewBag.Students = students
                    .OrderBy(s => s.FullName)
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = $"{s.FullName} ({s.StudentNumber})",
                        Selected = model.StudentId == s.Id
                    })
                    .ToList();

                List<CourseMiniDto> courses = model.StudentId > 0
                    ? await FetchAvailableCoursesForStudentAsync(model.StudentId, ct)
                    : await FetchCoursesAsync(ct);

                ViewBag.Courses = courses
                    .OrderBy(c => c.Code)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = $"{c.Code} - {c.Name}",
                        Selected = model.CourseId == c.Id
                    })
                    .ToList();

                return View(model);
            }

            try
            {
                // Varsayılan: POST /api/Enrollments (body: EnrollRequestDto)
                var resp = await _http.PostAsJsonAsync("Enrollments/enroll", model, _json, ct);

                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Öğrenci derse kaydedildi.";
                    return RedirectToAction(nameof(Index), new { studentId = model.StudentId });
                }

                var api = await SafeRead<ApiDataResult<object>>(resp, ct);
                TempData["Error"] = api?.Message ?? $"Kayıt başarısız ({(int)resp.StatusCode}).";

                // Hata durumunda dropdown’ları yeniden doldur
                var students = await FetchStudentsAsync(ct);
                ViewBag.Students = students
                    .OrderBy(s => s.FullName)
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = $"{s.FullName} ({s.StudentNumber})",
                        Selected = model.StudentId == s.Id
                    })
                    .ToList();

                List<CourseMiniDto> courses2 = model.StudentId > 0
                    ? await FetchAvailableCoursesForStudentAsync(model.StudentId, ct)
                    : await FetchCoursesAsync(ct);

                ViewBag.Courses = courses2
                    .OrderBy(c => c.Code)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = $"{c.Code} - {c.Name}",
                        Selected = model.CourseId == c.Id
                    })
                    .ToList();

                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return View(model);
            }
        }

        // -------- DELETE (senin kodun kalıyor) --------
        public IActionResult Delete(int studentId, int courseId)
        {
            ViewBag.StudentId = studentId;
            ViewBag.CourseId = courseId;
            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int studentId, int courseId, CancellationToken ct)
        {
            try
            {
                // ESKİ:
                // var resp = await _http.DeleteAsync($"Enrollments?studentId={studentId}&courseId={courseId}", ct);

                // YENİ: backend route’una göre:
                var resp = await _http.DeleteAsync($"Enrollments/unenroll?studentId={studentId}&courseId={courseId}", ct);
                // NOT: Eğer BaseAddress'in "http://localhost:5180/" ise -> "api/Enrollments/unenroll?..." yazmalısın.

                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Kayıt (enrollment) silindi.";
                    return RedirectToAction(nameof(Index), new { studentId });
                }

                var api = await SafeRead<ApiDataResult<object>>(resp, ct);
                TempData["Error"] = api?.Message ?? $"Silme başarısız ({(int)resp.StatusCode}).";
                return RedirectToAction(nameof(Index), new { studentId });
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Index), new { studentId });
            }
        }

        // -------- HELPERS --------
        private record StudentMiniDto(int Id, string StudentNumber, string FullName);

        private async Task<List<StudentMiniDto>> FetchStudentsAsync(CancellationToken ct)
        {
            var list = await _http.GetFromJsonAsync<List<StudentMiniDto>>("Students", _json, ct);
            if (list != null) return list;

            var wrapped = await _http.GetFromJsonAsync<ApiDataResult<List<StudentMiniDto>>>("Students", _json, ct);
            return wrapped?.Data ?? new List<StudentMiniDto>();
        }

        private async Task<List<CourseMiniDto>> FetchCoursesAsync(CancellationToken ct)
        {
            var list = await _http.GetFromJsonAsync<List<CourseMiniDto>>("Courses", _json, ct);
            if (list != null) return list;

            var wrapped = await _http.GetFromJsonAsync<ApiDataResult<List<CourseMiniDto>>>("Courses", _json, ct);
            return wrapped?.Data ?? new List<CourseMiniDto>();
        }

        // Öğrenciye zaten kayıtlı olduğu dersleri çıkar
        private async Task<List<CourseMiniDto>> FetchAvailableCoursesForStudentAsync(int studentId, CancellationToken ct)
        {
            var all = await FetchCoursesAsync(ct);

            // GET /api/Enrollments/student/{studentId}/courses
            var enrolled = await _http.GetFromJsonAsync<List<CourseMiniDto>>(
                $"Enrollments/student/{studentId}/courses", _json, ct) ?? new();

            var enrolledIds = new HashSet<int>(enrolled.Select(e => e.Id));
            return all.Where(c => !enrolledIds.Contains(c.Id)).ToList();
        }

        private static async Task<T?> SafeRead<T>(HttpResponseMessage resp, CancellationToken ct)
        {
            try { return await resp.Content.ReadFromJsonAsync<T>(_json, ct); }
            catch { return default; }
        }
    }
}
