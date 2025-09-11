using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentAutomationMVC.Web.Models.Grades;
using StudentAutomationMVC.Web.Models.Shared;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudentAutomationMVC.Web.Controllers
{
    public class GradesController : Controller
    {
        private readonly HttpClient _http;

        // IMPORTANT: String enum için converter ekledik
        private static readonly JsonSerializerOptions _json = CreateJson();
        private static JsonSerializerOptions CreateJson()
        {
            var o = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            o.Converters.Add(new JsonStringEnumConverter());
            return o;
        }

        public GradesController(IHttpClientFactory factory)
        {
            // Program.cs’te tanımlı named client
            _http = factory.CreateClient("UstaApi");
        }

        // GET: /Grades
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            try
            {
                // NOT: BaseAddress’ine göre şu satırı seç:
                // BaseAddress "http://localhost:5180/api/" ise:
                var list = await _http.GetFromJsonAsync<List<GradeListDto>>("Grades", _json, ct)
                           ?? new List<GradeListDto>();

                // Eğer BaseAddress "http://localhost:5180/" ise yukarıyı şu yap:
                // var list = await _http.GetFromJsonAsync<List<GradeListDto>>("api/Grades", _json, ct) ?? new();

                // Dropdown’lar (öğrenci/derse adla seçim)
                ViewBag.Students = await FetchStudentsSelectAsync(ct);
                ViewBag.Courses = await FetchCoursesSelectAsync(ct);

                return View(list);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return View(new List<GradeListDto>());
            }
        }

        // GET: /Grades/Details/5
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            try
            {
                var detail = await _http.GetFromJsonAsync<GradeDetailDto>($"Grades/{id}", _json, ct);
                // Eğer BaseAddress "http://localhost:5180/" ise: $"api/Grades/{id}"
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

        // GET: /Grades/Create
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await FillLookupAsync(ct);
            return View(new GradeUpsertDto { Type = GradeType.Midterm });
        }

        // POST: /Grades/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GradeUpsertDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await FillLookupAsync(ct);
                return View(model);
            }

            try
            {
                var resp = await _http.PostAsJsonAsync("Grades/upsert", model, _json, ct);
                // BaseAddress "http://localhost:5180/" ise: "api/Grades/upsert"
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Not eklendi/güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                var api = await SafeRead<ApiDataResult<object>>(resp, ct);
                TempData["Error"] = api?.Message ?? $"İşlem başarısız ({(int)resp.StatusCode}).";
                await FillLookupAsync(ct);
                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                await FillLookupAsync(ct);
                return View(model);
            }
        }

        // GET: /Grades/Edit/5  (Not: id ile detayı çekip Upsert formu dolduruyoruz)
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            try
            {
                var g = await _http.GetFromJsonAsync<GradeDetailDto>($"Grades/{id}", _json, ct);
                // BaseAddress "http://localhost:5180/" ise: $"api/Grades/{id}"
                if (g is null)
                {
                    TempData["Error"] = "Kayıt bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                var vm = new GradeUpsertDto
                {
                    StudentId = g.StudentId,
                    CourseId = g.CourseId,
                    Type = g.Type,
                    Value = g.Value,
                    Term = g.Term
                };

                await FillLookupAsync(ct);
                return View(vm);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Grades/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GradeUpsertDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await FillLookupAsync(ct);
                return View(model);
            }

            try
            {
                var resp = await _http.PostAsJsonAsync("Grades/upsert", model, _json, ct);
                // BaseAddress "http://localhost:5180/" ise: "api/Grades/upsert"
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Not güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                var api = await SafeRead<ApiDataResult<object>>(resp, ct);
                TempData["Error"] = api?.Message ?? $"Güncelleme başarısız ({(int)resp.StatusCode}).";
                await FillLookupAsync(ct);
                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                await FillLookupAsync(ct);
                return View(model);
            }
        }

        // GET: /Grades/Delete/5
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                var g = await _http.GetFromJsonAsync<GradeDetailDto>($"Grades/{id}", _json, ct);
                if (g is not null) return View(g);

                TempData["Error"] = "Kayıt bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Grades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                var resp = await _http.DeleteAsync($"Grades/{id}", ct);
                // BaseAddress "http://localhost:5180/" ise: $"api/Grades/{id}"
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Not silindi.";
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

        // ------- Yardımcılar (dropdown’lar + safe read) --------

        private async Task FillLookupAsync(CancellationToken ct)
        {
            ViewBag.Students = await FetchStudentsSelectAsync(ct);
            ViewBag.Courses = await FetchCoursesSelectAsync(ct);
        }

        private async Task<List<SelectListItem>> FetchStudentsSelectAsync(CancellationToken ct)
        {
            try
            {
                var minis = await _http.GetFromJsonAsync<List<StudentMiniDto>>("Students", _json, ct);
                // BaseAddress "http://localhost:5180/" ise: "api/Students"
                if (minis != null)
                {
                    return minis
                        .OrderBy(x => x.FullName)
                        .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.FullName} ({x.StudentNumber})" })
                        .ToList();
                }
            }
            catch { }

            try
            {
                var wrapped = await _http.GetFromJsonAsync<ApiDataResult<List<StudentMiniDto>>>("Students", _json, ct);
                if (wrapped?.Data != null)
                {
                    return wrapped.Data
                        .OrderBy(x => x.FullName)
                        .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.FullName} ({x.StudentNumber})" })
                        .ToList();
                }
            }
            catch { }

            return new();
        }

        private async Task<List<SelectListItem>> FetchCoursesSelectAsync(CancellationToken ct)
        {
            try
            {
                var minis = await _http.GetFromJsonAsync<List<CourseMiniDto>>("Courses", _json, ct);
                if (minis != null)
                {
                    return minis
                        .OrderBy(x => x.Code)
                        .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.Code} - {x.Name}" })
                        .ToList();
                }
            }
            catch { }

            try
            {
                var wrapped = await _http.GetFromJsonAsync<ApiDataResult<List<CourseMiniDto>>>("Courses", _json, ct);
                if (wrapped?.Data != null)
                {
                    return wrapped.Data
                        .OrderBy(x => x.Code)
                        .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.Code} - {x.Name}" })
                        .ToList();
                }
            }
            catch { }

            return new();
        }

        private static async Task<T?> SafeRead<T>(HttpResponseMessage resp, CancellationToken ct)
        {
            try { return await resp.Content.ReadFromJsonAsync<T>(_json, ct); }
            catch { return default; }
        }

        // DTO kısaları (UI için minimal)
        private record StudentMiniDto(int Id, string StudentNumber, string FullName);
        private record CourseMiniDto(int Id, string Code, string Name);
    }
}
