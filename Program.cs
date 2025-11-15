using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimals_api.Domain.DTOs;
using minimals_api.Domain.Interfaces;
using minimals_api.Domain.Services;
using minimals_api.Infra.Db;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IAdmService, AdmService>();
builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("mysql"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});
var app = builder.Build();

app.MapGet("/", () => "Olá pessoal");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdmService admService) =>
{
    if (admService.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso");
    else
        return Results.Unauthorized();
});


app.Run();