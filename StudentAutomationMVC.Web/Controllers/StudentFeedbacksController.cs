using Microsoft.AspNetCore.Mvc;
using StudentAutomationMVC.Web.Models.Shared;
using StudentAutomationMVC.Web.Models.Feedbacks;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudentAutomationMVC.Web.Controllers
{
    public class StudentFeedbacksController : Controller
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public StudentFeedbacksController(IHttpClientFactory factory)
        {
            // Program.cs'te "UstaApi" named client tanımlı olmalı
            _http = factory.CreateClient("UstaApi");
        }

        // GET: /StudentFeedbacks?courseId=2&studentId=1
        // En az bir filtre (courseId veya studentId) bekler.
        public async Task<IActionResult> Index(int? courseId, int? studentId, CancellationToken ct)
        {
            try
            {
                List<StudentFeedbackListDto> list = new();

                if (courseId is int cid)
                {
                    // GET /api/StudentFeedbacks/by-course/{courseId}
                    list = await _http.GetFromJsonAsync<List<StudentFeedbackListDto>>(
                               $"StudentFeedbacks/by-course/{cid}", _json, ct)
                           ?? new();
                }
                else if (studentId is int sid)
                {
                    // GET /api/StudentFeedbacks/by-student/{studentId}
                    list = await _http.GetFromJsonAsync<List<StudentFeedbackListDto>>(
                               $"StudentFeedbacks/by-student/{sid}", _json, ct)
                           ?? new();
                }
                else
                {
                    TempData["Error"] = "Listelemek için en az bir filtre verin (courseId veya studentId).";
                }

                ViewBag.CourseId = courseId;
                ViewBag.StudentId = studentId;

                return View(list);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return View(new List<StudentFeedbackListDto>());
            }
        }

        // GET: /StudentFeedbacks/Details/5
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            try
            {
                var fb = await GetByIdFlexibleAsync(id, ct);
                if (fb is null)
                {
                    TempData["Error"] = "Kayıt bulunamadı.";
                    return RedirectToIndexWithContext();
                }
                return View(fb);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToIndexWithContext();
            }
        }

        // GET: /StudentFeedbacks/Create?courseId=2&studentId=1
        public IActionResult Create(int? courseId, int? studentId)
        {
            var vm = new StudentFeedbackCreateDto
            {
                CourseId = courseId ?? 0,
                StudentId = studentId ?? 0
            };
            ViewBag.CourseId = courseId;
            ViewBag.StudentId = studentId;
            return View(vm);
        }

        // POST: /StudentFeedbacks/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentFeedbackCreateDto model, int? courseId, int? studentId, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CourseId = courseId;
                ViewBag.StudentId = studentId;
                return View(model);
            }

            try
            {
                var resp = await _http.PostAsJsonAsync("StudentFeedbacks", model, _json, ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Geri bildirim eklendi.";
                    return RedirectToIndexWithContext(model.CourseId, model.StudentId);
                }

                var api = await SafeRead<ApiDataResult<object>>(resp, ct);
                TempData["Error"] = api?.Message ?? $"Ekleme başarısız ({(int)resp.StatusCode}).";
                ViewBag.CourseId = courseId;
                ViewBag.StudentId = studentId;
                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                ViewBag.CourseId = courseId;
                ViewBag.StudentId = studentId;
                return View(model);
            }
        }

        // GET: /StudentFeedbacks/Edit/5
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            try
            {
                var fb = await GetByIdFlexibleAsync(id, ct);
                if (fb is null)
                {
                    TempData["Error"] = "Kayıt bulunamadı.";
                    return RedirectToIndexWithContext();
                }

                var vm = new StudentFeedbackUpdateDto
                {
                    Id = fb.Id,
                    Comment = fb.Comment
                };

                ViewBag.Meta = fb; // readonly gösterimler için
                return View(vm);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToIndexWithContext();
            }
        }

        // POST: /StudentFeedbacks/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentFeedbackUpdateDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                // Görsel bilgiler için tekrar doldur
                var fb = await GetByIdFlexibleAsync(model.Id, ct);
                ViewBag.Meta = fb;
                return View(model);
            }

            try
            {
                var resp = await _http.PutAsJsonAsync("StudentFeedbacks", model, _json, ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Geri bildirim güncellendi.";
                    // Bağlama geri dön (courseId/studentId bilen Details verisi varsa ona göre)
                    var fb = await GetByIdFlexibleAsync(model.Id, ct);
                    return RedirectToIndexWithContext(fb?.CourseId, fb?.StudentId);
                }

                var api = await SafeRead<ApiDataResult<object>>(resp, ct);
                TempData["Error"] = api?.Message ?? $"Güncelleme başarısız ({(int)resp.StatusCode}).";
                var meta = await GetByIdFlexibleAsync(model.Id, ct);
                ViewBag.Meta = meta;
                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                var meta = await GetByIdFlexibleAsync(model.Id, ct);
                ViewBag.Meta = meta;
                return View(model);
            }
        }

        // GET: /StudentFeedbacks/Delete/5
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                var fb = await GetByIdFlexibleAsync(id, ct);
                if (fb is not null) return View(fb);

                TempData["Error"] = "Kayıt bulunamadı.";
                return RedirectToIndexWithContext();
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToIndexWithContext();
            }
        }

        // POST: /StudentFeedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                // Silmeden önce listeye dönebilmek için course/student yakala
                var fb = await GetByIdFlexibleAsync(id, ct);

                var resp = await _http.DeleteAsync($"StudentFeedbacks/{id}", ct);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Geri bildirim silindi.";
                    return RedirectToIndexWithContext(fb?.CourseId, fb?.StudentId);
                }

                var api = await SafeRead<ApiDataResult<object>>(resp, ct);
                TempData["Error"] = api?.Message ?? $"Silme başarısız ({(int)resp.StatusCode}).";
                return RedirectToIndexWithContext(fb?.CourseId, fb?.StudentId);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye ulaşılamadı.";
                return RedirectToIndexWithContext();
            }
        }

        // --- Yardımcılar ---

        // API dokümanında GET /StudentFeedbacks/{id} açıkça listelenmemiş olsa da
        // öncelikle orayı dener; 404/uyuşmazlık olursa null döner.
        private async Task<StudentFeedbackListDto?> GetByIdFlexibleAsync(int id, CancellationToken ct)
        {
            // 1) Düz dene: /StudentFeedbacks/{id} -> StudentFeedbackListDto
            try
            {
                var plain = await _http.GetFromJsonAsync<StudentFeedbackListDto>($"StudentFeedbacks/{id}", _json, ct);
                if (plain is not null) return plain;
            }
            catch { /* sarılı dene */ }

            // 2) Sarılı dene: ApiDataResult<StudentFeedbackListDto>
            try
            {
                var wrapped = await _http.GetFromJsonAsync<ApiDataResult<StudentFeedbackListDto>>($"StudentFeedbacks/{id}", _json, ct);
                if (wrapped?.Success == true && wrapped.Data is not null) return wrapped.Data;
            }
            catch { /* id ile erişim yoksa null döneceğiz */ }

            return null;
        }

        private IActionResult RedirectToIndexWithContext(int? courseId = null, int? studentId = null)
        {
            if (courseId is not null) return RedirectToAction(nameof(Index), new { courseId });
            if (studentId is not null) return RedirectToAction(nameof(Index), new { studentId });
            return RedirectToAction(nameof(Index));
        }

        private static async Task<T?> SafeRead<T>(HttpResponseMessage resp, CancellationToken ct)
        {
            try { return await resp.Content.ReadFromJsonAsync<T>(_json, ct); }
            catch { return default; }
        }
    }
}
