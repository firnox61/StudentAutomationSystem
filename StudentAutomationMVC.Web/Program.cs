using StudentAutomationMVC.Web.Security;

var builder = WebApplication.CreateBuilder(args);





builder.Services.AddHttpContextAccessor();

// 2) Handler’ý DI’a ekle
builder.Services.AddTransient<StudentAutomationMVC.Web.Security.BearerTokenHandler>();


// DI container'a HttpClientFactory ekle
builder.Services.AddHttpClient("UstaApi", c =>
{
    c.BaseAddress = new Uri("http://localhost:5180/api/"); // PROD'da https
})
.AddHttpMessageHandler<BearerTokenHandler>();

// ---------- Auth ----------
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Auth/Login";        // Yetkisizse buraya yönlendir
        options.AccessDeniedPath = "/Auth/Login"; // Eriþim yoksa
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;         // Kullanýcý aktifse süreyi yeniler
    });

builder.Services.AddAuthorization();

















// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // önce authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();