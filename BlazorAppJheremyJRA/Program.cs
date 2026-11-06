using BlazorAppJheremyJRA;
using BlazorAppJheremyJRA.Entities;
using BlazorAppJheremyJRA.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ---------- Permisos por rol ----------
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy(Politicas.Ver, p => p.RequireClaim("permiso", Permisos.Ver));
    options.AddPolicy(Politicas.Crear, p => p.RequireClaim("permiso", Permisos.Crear));
    options.AddPolicy(Politicas.Editar, p => p.RequireClaim("permiso", Permisos.Editar));
    options.AddPolicy(Politicas.Eliminar, p => p.RequireClaim("permiso", Permisos.Eliminar));
    options.AddPolicy(Politicas.AdministrarUsuarios, p => p.RequireClaim("permiso", Permisos.AdministrarUsuarios));
});

// ---------- Autenticación ----------
builder.Services.AddScoped<TokenStore>();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddScoped<AuthService>();

// ---------- Tus servicios ----------
builder.Services.AddScoped<UsuarioAdminService>();
builder.Services.AddScoped<TipoLibroService>();
builder.Services.AddScoped<LibroService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<AutorService>();

// HttpClient que envía el token en cada petición a la API
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthHeaderHandler>();
    handler.InnerHandler = new HttpClientHandler();

    return new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:5273/")
    };
});

await builder.Build().RunAsync();