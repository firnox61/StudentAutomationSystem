using Microsoft.AspNetCore.Mvc;
using StudentAutomationMVC.Web.Models.Shared;
using StudentAutomationMVC.Web.Models.Teachers;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudentAutomationMVC.Web.Controllers
{
    public class TeachersController : Controller
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public TeachersController(IHttpClientFactory factory)
        {
            // Program.cs'te tanımlanacak "UstaApi" adlı named client
            _http = factory.CreateClient("UstaApi");
        }

        // GET: /Teachers
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            try
            {
                var list = await _http.GetFromJsonAsync<List<TeacherListDto>>("Teachers", _json, ct)
                           ?? new List<TeacherListDto>();
                return View(list);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return View(new List<TeacherListDto>());
            }
        }

        // GET: /Teachers/Details/5
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            try
            {
                var detail = await _http.GetFromJsonAsync<TeacherDetailDto>($"Teachers/{id}", _json, ct);
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

        // GET: /Teachers/Create
        public IActionResult Create() => View(new TeacherCreateDto());

        // POST: /Teachers/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherCreateDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var resp = await _http.PostAsJsonAsync("Teachers", model, _json, ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Öğretmen oluşturuldu.";
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

        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            try
            {
                var t = await GetOneFlexibleAsync<StudentAutomationMVC.Web.Models.Teachers.TeacherDetailDto>($"Teachers/{id}", ct);
                if (t is null)
                {
                    TempData["Error"] = "Kayıt bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                var vm = new StudentAutomationMVC.Web.Models.Teachers.TeacherUpdateDto
                {
                    Id = t.Id,
                    UserId = 0, // Detail DTO UserId yoksa UI'dan seçilecek ya da ayrı endpointten doldurulacak
                    Title = t.Title,
                    Department = t.Department,
                    Status = t.Status
                };
                return View(vm);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Teachers/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TeacherUpdateDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var resp = await _http.PutAsJsonAsync("Teachers", model, _json, ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Öğretmen güncellendi.";
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

        // GET: /Teachers/Delete/5
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                var t = await GetOneFlexibleAsync<StudentAutomationMVC.Web.Models.Teachers.TeacherDetailDto>($"Teachers/{id}", ct);
                if (t is not null) return View(t);

                TempData["Error"] = "Kayıt bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                var resp = await _http.DeleteAsync($"Teachers/{id}", ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Öğretmen silindi.";
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
        // Controller sınıfı içine ekle (private method)
        private async Task<T?> GetOneFlexibleAsync<T>(string url, CancellationToken ct)
        {
            // 1) Düz DTO dene: { ... tüm alanlar ... }
            try
            {
                var plain = await _http.GetFromJsonAsync<T>(url, _json, ct);
                if (plain is not null) return plain;
            }
            catch { /* yok say, sarılı dene */ }

            // 2) Sarılı tip dene: { "success": true, "message": "...", "data": { ... } }
            try
            {
                var wrapped = await _http.GetFromJsonAsync<StudentAutomationMVC.Web.Models.Shared.ApiDataResult<T>>(url, _json, ct);
                if (wrapped?.Success == true && wrapped.Data is not null) return wrapped.Data;
            }
            catch { /* her ikisi de olmazsa null döneceğiz */ }

            return default;
        }
        // ---- küçük yardımcı ----
        private static async Task<T?> SafeRead<T>(HttpResponseMessage resp, CancellationToken ct)
        {
            try { return await resp.Content.ReadFromJsonAsync<T>(_json, ct); }
            catch { return default; }
        }
    }
}
