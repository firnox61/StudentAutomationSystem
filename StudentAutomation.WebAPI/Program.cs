using Autofac;
using Autofac.Extensions.DependencyInjection;
using EFCore.NamingConventions;  // <-- BU gerekli
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentAutomation.Application.MappingProfiles;
using StudentAutomation.Core.Utilities.IoC;
using StudentAutomation.Infrastructure.Persistence.Context;
using StudentAutomation.Infrastructure.Security.Encryption;
using StudentAutomation.Infrastructure.Security.Jwt;
using StudentAutomation.WebAPI.DependencyInjection;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);





Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: Path.Combine(AppContext.BaseDirectory, "logs", "log-.txt"),
        rollingInterval: RollingInterval.Day,
        shared: true,                 // <<< çok kritik: paylaşımlı erişim
        retainedFileCountLimit: 10,
        flushToDiskInterval: TimeSpan.FromSeconds(1))
    .CreateLogger();

builder.Host.UseSerilog();


// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


// Swagger (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npg =>
        {
            npg.EnableRetryOnFailure(
                maxRetryCount: 5,                // en fazla 5 kez dene
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);
        })
        .UseSnakeCaseNamingConvention(); // opsiyonel
    // options.CommandTimeout(30); // istersen buradan sorgu timeout'u da ayarlayabilirsin
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(GeneralMapping).Assembly);

// Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(opt =>
{
    opt.RegisterModule(new AutofacBusinessModule());
});

// CORS — tek policy yeterli
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5135")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// JWT
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey),
            // ÖNEMLİ: JWT'de "role" adını rol olarak kullan
           // RoleClaimType = "role",

            // İsteğe bağlı ama netlik için önerilir: "name" → User.Identity.Name
           // NameClaimType = "name",

          //  ClockSkew = TimeSpan.Zero
              RoleClaimType = ClaimTypes.Role,
             NameClaimType = ClaimTypes.NameIdentifier,
              ClockSkew = TimeSpan.Zero
        };
    });

// Core module (sizin DI altyapınız)
builder.Services.AddDependencyResolvers(new ICoreModule[] { new CoreModule() });

var app = builder.Build();

// HATA AYIKLAMA: Development’ta Swagger UI aç
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // -> /swagger
}
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});
// Statik dosyalar (gerekliyse)
app.UseStaticFiles();

// Sıra: CORS -> Auth -> Authorization
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Controller endpointleri
app.MapControllers();
// 🔹 Migration'ları otomatik uygula
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();
}

app.Run();
