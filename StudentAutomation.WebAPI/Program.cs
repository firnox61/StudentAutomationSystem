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

var builder = WebApplication.CreateBuilder(args);

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
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention()); // opsiyonel
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

// Statik dosyalar (gerekliyse)
app.UseStaticFiles();

// Sıra: CORS -> Auth -> Authorization
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Controller endpointleri
app.MapControllers();

app.Run();
