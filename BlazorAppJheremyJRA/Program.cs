using BlazorAppJheremyJRA;
using BlazorAppJheremyJRA.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder =
WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

builder.RootComponents.Add<HeadOutlet>(
"head::after"
);

builder.Services.AddScoped<TipoLibroService>();

builder.Services.AddScoped<LibroService>();

builder.Services.AddScoped<CategoriaService>();

builder.Services.AddScoped<AutorService>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5273/")
});

await builder.Build().RunAsync();
