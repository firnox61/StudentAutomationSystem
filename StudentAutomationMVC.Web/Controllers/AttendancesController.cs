using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentAutomationMVC.Web.Models.Attendances;
using StudentAutomationMVC.Web.Models.Shared;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudentAutomationMVC.Web.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public AttendancesController(IHttpClientFactory factory)
        {
            // Program.cs'de base address'i /api/ olacak şekilde "UstaApi" named client tanımlı
            _http = factory.CreateClient("UstaApi");
        }

        // GET: /Attendances
        // Filtrelenebilir liste: courseId, studentId, date (opsiyonel)
        public async Task<IActionResult> Index(int? courseId, int? studentId, DateOnly? date, CancellationToken ct)
        {
            // Filtre select'leri için öğrenciler ve dersler
            ViewBag.Students = await LoadStudentsAsync(ct);
            ViewBag.Courses = await LoadCoursesAsync(ct);

            var list = new List<AttendanceListDto>();

            try
            {
                if (courseId is int cid)
                {
                    // /Attendances/by-course/{courseId}?date=...
                    var url = $"Attendances/by-course/{cid}";
                    if (date.HasValue) url += $"?date={date:yyyy-MM-dd}";
                    list = await _http.GetFromJsonAsync<List<AttendanceListDto>>(url, _json, ct)
                           ?? new List<AttendanceListDto>();
                }
                else if (studentId is int sid)
                {
                    // /Attendances/by-student?studentId=...&courseId=...
                    var url = $"Attendances/by-student?studentId={sid}";
                    if (courseId.HasValue) url += $"&courseId={courseId.Value}";
                    list = await _http.GetFromJsonAsync<List<AttendanceListDto>>(url, _json, ct)
                           ?? new List<AttendanceListDto>();
                }
                // aksi halde boş liste – kullanıcı filtre seçsin
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
            }

            return View(list);
        }

        // GET: /Attendances/Create
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            ViewBag.Students = await LoadStudentsAsync(ct);
            ViewBag.Courses = await LoadCoursesAsync(ct);

            var vm = new AttendanceUpsertDto
            {
                Date = DateOnly.FromDateTime(DateTime.Today),
                Status = AttendanceStatus.Present
            };
            return View("Upsert", vm);
        }

        // POST: /Attendances/Create  (API: POST Attendances/upsert)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttendanceUpsertDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Students = await LoadStudentsAsync(ct);
                ViewBag.Courses = await LoadCoursesAsync(ct);
                return View("Upsert", model);
            }

            try
            {
                var resp = await _http.PostAsJsonAsync("Attendances/upsert", model, _json, ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Yoklama kaydı oluşturuldu / güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                var api = await SafeRead<ApiDataResult<object>>(resp, ct);
                TempData["Error"] = api?.Message ?? $"Kayıt başarısız ({(int)resp.StatusCode}).";
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
            }

            ViewBag.Students = await LoadStudentsAsync(ct);
            ViewBag.Courses = await LoadCoursesAsync(ct);
            return View("Upsert", model);
        }

        // GET: /Attendances/Edit/5  (API: GET Attendances/{id})
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            try
            {
                var d = await GetOneFlexibleAsync<AttendanceDetailDto>($"Attendances/{id}", ct);
                if (d is null)
                {
                    TempData["Error"] = "Kayıt bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                var vm = new AttendanceUpsertDto
                {
                    StudentId = d.StudentId,
                    CourseId = d.CourseId,
                    Date = d.Date,
                    Week = d.Week,
                    Status = d.Status,
                    Note = d.Note
                };

                ViewBag.Students = await LoadStudentsAsync(ct);
                ViewBag.Courses = await LoadCoursesAsync(ct);
                ViewBag.EditId = id; // View'de bilgi amaçlı
                return View("Upsert", vm);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Attendances/Edit/5  (API yine: POST Attendances/upsert)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AttendanceUpsertDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Students = await LoadStudentsAsync(ct);
                ViewBag.Courses = await LoadCoursesAsync(ct);
                ViewBag.EditId = id;
                return View("Upsert", model);
            }

            try
            {
                // Upsert: aynı kombinasyon (StudentId, CourseId, Date) varsa günceller
                var resp = await _http.PostAsJsonAsync("Attendances/upsert", model, _json, ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Yoklama kaydı güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                var api = await SafeRead<ApiDataResult<object>>(resp, ct);
                TempData["Error"] = api?.Message ?? $"Güncelleme başarısız ({(int)resp.StatusCode}).";
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
            }

            ViewBag.Students = await LoadStudentsAsync(ct);
            ViewBag.Courses = await LoadCoursesAsync(ct);
            ViewBag.EditId = id;
            return View("Upsert", model);
        }

        // GET: /Attendances/Details/5   (API: GET Attendances/{id})
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            try
            {
                var d = await GetOneFlexibleAsync<AttendanceDetailDto>($"Attendances/{id}", ct);
                if (d is null)
                {
                    TempData["Error"] = "Kayıt bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(d);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Attendances/Delete/5
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                var d = await GetOneFlexibleAsync<AttendanceDetailDto>($"Attendances/{id}", ct);
                if (d is null)
                {
                    TempData["Error"] = "Kayıt bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(d);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Attendances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                var resp = await _http.DeleteAsync($"Attendances/{id}", ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Yoklama kaydı silindi.";
                    return RedirectToAction(nameof(Index));
                }

                var api = await SafeRead<ApiDataResult<object>>(resp, ct);
                TempData["Error"] = api?.Message ?? $"Silme başarısız ({(int)resp.StatusCode}).";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ---------- Helpers ----------

        // Düz ya da ApiDataResult<T> sarmalı response'u esnekçe okur
        private async Task<T?> GetOneFlexibleAsync<T>(string url, CancellationToken ct)
        {
            try
            {
                var plain = await _http.GetFromJsonAsync<T>(url, _json, ct);
                if (plain is not null) return plain;
            }
            catch { /* sarılı dene */ }

            try
            {
                var wrapped = await _http.GetFromJsonAsync<ApiDataResult<T>>(url, _json, ct);
                if (wrapped?.Success == true && wrapped.Data is not null) return wrapped.Data;
            }
            catch { /* her ikisi de olmazsa null */ }

            return default;
        }

        private static async Task<T?> SafeRead<T>(HttpResponseMessage resp, CancellationToken ct)
        {
            try { return await resp.Content.ReadFromJsonAsync<T>(_json, ct); }
            catch { return default; }
        }

        // Öğrenci ve Ders select'leri — isimleri göster (id değil)
        private async Task<List<SelectListItem>> LoadStudentsAsync(CancellationToken ct)
        {
            // Beklenen DTO: StudentMiniDto { Id, StudentNumber, FullName }
            try
            {
                var minis = await _http.GetFromJsonAsync<List<StudentMiniDto>>("Students/minis", _json, ct);
                if (minis is not null)
                    return minis
                        .OrderBy(x => x.FullName)
                        .Select(x => new SelectListItem($"{x.FullName} ({x.StudentNumber})", x.Id.ToString()))
                        .ToList();
            }
            catch { /* endpoint farklıysa aşağıdaki alternatifi deneyebilirsin */ }

            // Alternatif: /Students gibi tam listeden map'lenebilir (gerektiğinde uyarlarsın)
            return new();
        }

        private async Task<List<SelectListItem>> LoadCoursesAsync(CancellationToken ct)
        {
            // Beklenen: Course listesinde Code + Name olsun ki isimle gösterelim
            try
            {
                var list = await _http.GetFromJsonAsync<List<CourseListMiniDto>>("Courses/minis", _json, ct);
                if (list is not null)
                    return list
                        .OrderBy(x => x.Code)
                        .Select(x => new SelectListItem($"{x.Code} — {x.Name}", x.Id.ToString()))
                        .ToList();
            }
            catch { /* alternatif aşağıda */ }

            try
            {
                var list = await _http.GetFromJsonAsync<List<CourseListMiniDto>>("Courses", _json, ct);
                if (list is not null)
                    return list
                        .OrderBy(x => x.Code)
                        .Select(x => new SelectListItem($"{x.Code} — {x.Name}", x.Id.ToString()))
                        .ToList();
            }
            catch { }

            return new();
        }

        // Bu mini DTO’ları MVC tarafında küçük modeller olarak kullanıyoruz (sadece select doldurmak için)
        private sealed class StudentMiniDto
        {
            public int Id { get; set; }
            public string StudentNumber { get; set; } = default!;
            public string FullName { get; set; } = default!;
        }

        private sealed class CourseListMiniDto
        {
            public int Id { get; set; }
            public string Code { get; set; } = default!;
            public string Name { get; set; } = default!;
        }
    }
}
