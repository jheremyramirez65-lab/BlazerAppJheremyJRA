using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BlazorAppJheremyJRA.API.Models;
using BlazorAppJheremyJRA.API.Services;
using BlazorAppJheremyJRA.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<BdbibliotecaContext>(options =>
    options.UseSqlServer(
        "Server=(localdb)\\MSSQLLocalDB;Database=BDBIBLIOTECA;Integrated Security=True;Encrypt=False;TrustServerCertificate=True"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:5094",
                  "https://localhost:7250",
                  "http://localhost:53794",
                  "https://localhost:44366")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ---------- Autenticación JWT ----------
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "Configura Jwt:Key (mínimo 32 caracteres) con user-secrets o en appsettings.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });

// ---------- Permisos por rol ----------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Politicas.Ver, p => p.RequireClaim("permiso", Permisos.Ver));
    options.AddPolicy(Politicas.Crear, p => p.RequireClaim("permiso", Permisos.Crear));
    options.AddPolicy(Politicas.Editar, p => p.RequireClaim("permiso", Permisos.Editar));
    options.AddPolicy(Politicas.Eliminar, p => p.RequireClaim("permiso", Permisos.Eliminar));
    options.AddPolicy(Politicas.AdministrarUsuarios, p => p.RequireClaim("permiso", Permisos.AdministrarUsuarios));
});

builder.Services.AddScoped<JwtService>();

// ---------- Límite de intentos (anti fuerza bruta) ----------
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocido",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();