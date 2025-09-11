using Microsoft.AspNetCore.Mvc;
using StudentAutomationMVC.Web.Models;
using StudentAutomationMVC.Web.Models.Users;
using System.Diagnostics;
using System.Text.Json;

namespace StudentAutomationMVC.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _api;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory factory)
        {
            _logger = logger;
            _api = factory.CreateClient("UstaApi");
            _api.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
