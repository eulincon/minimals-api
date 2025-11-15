using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimals_api.Domain.DTOs;
using minimals_api.Domain.Interfaces;
using minimals_api.Domain.ModelViews;
using minimals_api.Domain.Services;
using minimals_api.Infra.Db;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IAdmService, AdmService>();
builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("mysql"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.MapGet("/", () => TypedResults.Redirect("/swagger"));

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdmService admService) =>
{
    if (admService.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso");
    else
        return Results.Unauthorized();
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();