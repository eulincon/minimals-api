using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimals_api.Domain.DTOs;
using minimals_api.Domain.Entities;
using minimals_api.Domain.Interfaces;
using minimals_api.Domain.ModelViews;
using minimals_api.Domain.Services;
using minimals_api.Infra.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IAdmService, AdmService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("mysql"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => TypedResults.Redirect("/swagger")).WithTags("Home");
#endregion

#region Adm
app.MapPost("/adm/login", ([FromBody] LoginDTO loginDTO, IAdmService admService) =>
{
    if (admService.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso");
    else
        return Results.Unauthorized();
}).WithTags("Adms");
#endregion

#region Vehicle
app.MapPost("/vechiles", ([FromBody] VehicleDTO vehicleDto, IVehicleService vehicleService) =>
{
    var vehicle = new Vehicle
    {
        Nome = vehicleDto.Nome,
        Marca = vehicleDto.Marca,
        Ano = vehicleDto.Ano
    };
    vehicleService.Add(vehicle);
    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
}).WithTags("Vehicles");
app.MapGet("/vechicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.All(page);
    return Results.Ok(vehicles);
}).WithTags("Vehicles");
app.MapPut("/vechicles", ([FromQuery] int id, VehicleDTO vehicleDto, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.FindById(id);
    if (vehicle == null) return Results.NotFound();

    vehicle.Nome = vehicleDto.Nome;
    vehicle.Ano = vehicleDto.Ano;
    vehicle.Marca = vehicleDto.Marca;

    vehicleService.Update(vehicle);
    return Results.Ok(vehicle);
}).WithTags("Vehicles");
app.MapDelete("/vechicles", ([FromQuery] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.FindById(id);
    if (vehicle == null) return Results.NotFound();

    vehicleService.Delete(vehicle);
    return Results.NoContent();
}).WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion