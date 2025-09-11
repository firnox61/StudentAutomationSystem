using Microsoft.AspNetCore.Mvc;
using StudentAutomationMVC.Web.Models.Shared;
using StudentAutomationMVC.Web.Models.Courses;
using StudentAutomationMVC.Web.Models.Students;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudentAutomationMVC.Web.Controllers
{
    public class CoursesController : Controller
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public CoursesController(IHttpClientFactory factory)
        {
            // Program.cs'te "UstaApi" named client olmalı
            _http = factory.CreateClient("UstaApi");
        }

        // GET: /Courses
        public async Task<IActionResult> Index(bool? onlyActive, CancellationToken ct)
        {
            try
            {
                var url = onlyActive is null ? "Courses" : $"Courses?onlyActive={onlyActive.Value.ToString().ToLower()}";
                var list = await _http.GetFromJsonAsync<List<CourseListDto>>(url, _json, ct) ?? new List<CourseListDto>();
                ViewBag.OnlyActive = onlyActive;

                // EnrollmentCount 0 gelenler için detaydan gerçek sayıyı çekip doldur
                var toFix = list.Where(c => c.EnrollmentCount == 0).Take(50).ToList(); // koruyucu limit
                if (toFix.Count > 0)
                {
                    await Task.WhenAll(toFix.Select(async c =>
                    {
                        try
                        {
                            var detail = await GetOneFlexibleAsync<CourseDetailDto>($"Courses/{c.Id}", ct);
                            if (detail != null && detail.EnrollmentCount > 0)
                                c.EnrollmentCount = detail.EnrollmentCount;
                        }
                        catch { /* yoksay */ }
                    }));
                }

                return View(list);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return View(new List<CourseListDto>());
            }
        }


        // GET: /Courses/Details/5
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            try
            {
                var detail = await GetOneFlexibleAsync<CourseDetailDto>($"Courses/{id}", ct);
                if (detail == null)
                {
                    TempData["Error"] = "Kayıt bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(detail);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Courses/Create
        public IActionResult Create() => View(new CourseCreateDto());

        // POST: /Courses/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var resp = await _http.PostAsJsonAsync("Courses", model, _json, ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Ders oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }

                var api = await SafeRead<ApiDataResult<object>>(resp, ct);
                TempData["Error"] = api?.Message ?? $"Ekleme başarısız ({(int)resp.StatusCode}).";
                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return View(model);
            }
        }

        // GET: /Courses/Edit/5
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            try
            {
                var c = await GetOneFlexibleAsync<CourseDetailDto>($"Courses/{id}", ct);
                if (c is null)
                {
                    TempData["Error"] = "Kayıt bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                // CourseDetailDto'da Credits YOK; bu yüzden burada set edemiyoruz.
                // Kullanıcı Edit formunda yeni Credits değerini girecek.
                var vm = new CourseUpdateDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    TeacherId = c.TeacherId,
                    // Credits -> backend bu bilgiyi Detail'de döndürmüyor; kullanıcı dolduracak.
                };
                return View(vm);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Courses/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseUpdateDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var resp = await _http.PutAsJsonAsync("Courses", model, _json, ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Ders güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                var api = await SafeRead<ApiDataResult<object>>(resp, ct);
                TempData["Error"] = api?.Message ?? $"Güncelleme başarısız ({(int)resp.StatusCode}).";
                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return View(model);
            }
        }

        // GET: /Courses/Delete/5
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                var c = await GetOneFlexibleAsync<CourseDetailDto>($"Courses/{id}", ct);
                if (c is not null) return View(c);

                TempData["Error"] = "Kayıt bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                var resp = await _http.DeleteAsync($"Courses/{id}", ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Ders silindi.";
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

        // ---- Ek aksiyonlar ----

        // Öğretmen atama
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTeacher(int courseId, int? teacherId, CancellationToken ct)
        {
            // 0 veya null ise atama yapma (öğretmeni yanlışlıkla silme riskini kaldır)
            if (teacherId is null || teacherId <= 0)
            {
                TempData["Error"] = "Geçerli bir Öğretmen Id giriniz.";
                return RedirectToAction(nameof(Details), new { id = courseId });
            }

            try
            {
                // 1) Body ile dene (camel+pascal key’ler birlikte)
                var payload = new { teacherId, TeacherId = teacherId };
                var resp = await _http.PutAsJsonAsync($"Courses/{courseId}/assign-teacher", payload, _json, ct);

                // 2) Olmazsa, bazı API’ler query-string bekler: .../assign-teacher?teacherId=#
                if (!resp.IsSuccessStatusCode)
                {
                    var qsUrl = $"Courses/{courseId}/assign-teacher?teacherId={teacherId}";
                    resp = await _http.PutAsJsonAsync(qsUrl, new { }, _json, ct);
                }

                TempData[resp.IsSuccessStatusCode ? "Success" : "Error"] =
                    resp.IsSuccessStatusCode ? "Öğretmen atandı." :
                    (await SafeRead<ApiDataResult<object>>(resp, ct))?.Message ?? "Atama başarısız.";
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
            }

            return RedirectToAction(nameof(Details), new { id = courseId });
        }

        // Aktif/Pasif
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(int courseId, bool isActive, CancellationToken ct)
        {
            try
            {
                // Hem camelCase hem PascalCase anahtarları birlikte gönderiyoruz (API hangisini bağlarsa onu alır)
                var payload = new { isActive, IsActive = isActive, active = isActive, Active = isActive };

                var resp = await _http.PutAsJsonAsync($"Courses/{courseId}/set-active", payload, _json, ct);

                TempData[resp.IsSuccessStatusCode ? "Success" : "Error"] =
                    resp.IsSuccessStatusCode ? "Durum güncellendi." :
                    (await SafeRead<ApiDataResult<object>>(resp, ct))?.Message ?? "Güncelleme başarısız.";
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
            }

            return RedirectToAction(nameof(Details), new { id = courseId });
        }

        // Kurs öğrencileri
        public async Task<IActionResult> Students(int courseId, CancellationToken ct)
        {
            try
            {
                // Esnek okuma: düz liste ya da { success, data } sarmalı
                var list = await GetOneFlexibleAsync<List<StudentMiniDto>>($"Courses/{courseId}/students", ct) ?? new();

                ViewBag.CourseId = courseId;
                return View(list);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Details), new { id = courseId });
            }
        }

        // ---- küçük yardımcılar ----
        private async Task<T?> GetOneFlexibleAsync<T>(string url, CancellationToken ct)
        {
            try
            {
                var plain = await _http.GetFromJsonAsync<T>(url, _json, ct);
                if (plain is not null) return plain;
            }
            catch { }

            try
            {
                var wrapped = await _http.GetFromJsonAsync<ApiDataResult<T>>(url, _json, ct);
                if (wrapped?.Success == true && wrapped.Data is not null) return wrapped.Data;
            }
            catch { }

            return default;
        }

        private static async Task<T?> SafeRead<T>(HttpResponseMessage resp, CancellationToken ct)
        {
            try { return await resp.Content.ReadFromJsonAsync<T>(_json, ct); }
            catch { return default; }
        }
    }
}
