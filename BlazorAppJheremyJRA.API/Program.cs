using Microsoft.EntityFrameworkCore;
using BlazorAppJheremyJRA.API.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<BdbibliotecaContext>(options =>
options.UseSqlServer(
"Server=(localdb)\\MSSQLLocalDB;Database=BDBIBLIOTECA;Integrated Security=True;Encrypt=False;TrustServerCertificate=True"
)
//options.UseSqlServer(
//    "Server=(localdb)\\MSSQLLocalDB;Database=BDBIBLIOTECA;Integrated Security=True;Encrypt=False;TrustServerCertificate=True"
//)

);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();