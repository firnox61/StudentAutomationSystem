using Microsoft.AspNetCore.Mvc;
using StudentAutomationMVC.Web.Models.Shared;
using StudentAutomationMVC.Web.Models.Students;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudentAutomationMVC.Web.Controllers
{
    public class StudentsController : Controller
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public StudentsController(IHttpClientFactory factory)
        {
            // Program.cs: builder.Services.AddHttpClient("UstaApi", c => c.BaseAddress = new Uri("http://localhost:5180/api/"));
            _http = factory.CreateClient("UstaApi");
        }

        // GET: /Students
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            try
            {
                var list = await _http.GetFromJsonAsync<List<StudentListDto>>("Students", _json, ct)
                           ?? new List<StudentListDto>();
                return View(list);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return View(new List<StudentListDto>());
            }
        }

        // GET: /Students/Details/5
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            try
            {
                // API tekil dönüyorsa doğrudan DTO iste
                var detail = await _http.GetFromJsonAsync<StudentDetailDto>($"Students/{id}", _json, ct);
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

        // GET: /Students/Create
        public IActionResult Create() => View(new StudentCreateDto());

        // POST: /Students/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentCreateDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var resp = await _http.PostAsJsonAsync("Students", model, _json, ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Öğrenci oluşturuldu.";
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

        // GET: /Students/Edit/5
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            try
            {
                var s = await _http.GetFromJsonAsync<StudentDetailDto>($"Students/{id}", _json, ct);
                if (s is null)
                {
                    TempData["Error"] = "Kayıt bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                var vm = new StudentUpdateDto
                {
                    Id = s.Id,
                    UserId = 0,                 // detail dönmüyorsa UI’dan seçilecek
                    StudentNumber = s.StudentNumber,
                    Department = s.Department,
                    BirthDate = s.BirthDate,
                    Status = s.Status
                };
                return View(vm);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Students/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentUpdateDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var resp = await _http.PutAsJsonAsync("Students", model, _json, ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Öğrenci güncellendi.";
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


        // GET: /Students/Delete/5
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                var s = await _http.GetFromJsonAsync<StudentDetailDto>($"Students/{id}", _json, ct);
                if (s is not null) return View(s);

                TempData["Error"] = "Kayıt bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                var resp = await _http.DeleteAsync($"Students/{id}", ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Öğrenci silindi.";
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

        // ---- küçük yardımcı ----
        private static async Task<T?> SafeRead<T>(HttpResponseMessage resp, CancellationToken ct)
        {
            try { return await resp.Content.ReadFromJsonAsync<T>(_json, ct); }
            catch { return default; }
        }
    }
}
