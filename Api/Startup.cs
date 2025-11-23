using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimals_api;
using minimals_api.Domain.DTOs;
using minimals_api.Domain.Entities;
using minimals_api.Domain.Interfaces;
using minimals_api.Domain.ModelViews;
using minimals_api.Domain.Services;
using minimals_api.Infra.Db;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration.GetSection("Jwt").ToString() ?? "";
    }

    public IConfiguration Configuration { get; set; }
    private string key;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdmService, AdmService>();
        services.AddScoped<IVehicleService, VehicleService>();

        services.AddDbContext<DbContexto>(options =>
        {
            options.UseMySql(Configuration.GetConnectionString("Mysql"),
            ServerVersion.AutoDetect(Configuration.GetConnectionString("Mysql")));
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insert JWT token here:"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                    },
                    new string[]{}
                }
            });
        });
    }

    #region Validation Helper
    static IResult? ValidateDTO<T>(T dto) where T : class
    {
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);

        if (!isValid)
        {
            var errorMessages = results.Select(r => new { field = r.MemberNames.FirstOrDefault() ?? "General", message = r.ErrorMessage });
            return Results.BadRequest(new { errors = errorMessages });
        }
        return null;
    }
    #endregion

    string GerarTokenJwt(Adm adm)
    {
        if (string.IsNullOrEmpty(key)) return string.Empty;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>()
    {
        new Claim("Email", adm.Email),
        new Claim(ClaimTypes.Role, adm.Perfil),
        new Claim("Perfil", adm.Perfil)
    };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoint =>
        {
            #region Home
            endpoint.MapGet("/", () => TypedResults.Redirect("/swagger")).AllowAnonymous().WithTags("Home");
            #endregion

            #region Adm
            endpoint.MapPost("/adms/login", ([FromBody] LoginDTO loginDTO, IAdmService admService) =>
            {
                var validationResult = ValidateDTO(loginDTO);
                if (validationResult != null)
                    return validationResult;

                var adm = admService.Login(loginDTO);
                if (adm != null)
                {
                    string token = GerarTokenJwt(adm);
                    return Results.Ok(new AdmLogado
                    {
                        Email = adm.Email,
                        Perfil = adm.Perfil,
                        Token = token
                    });
                }
                else
                    return Results.Unauthorized();
            })
            .AllowAnonymous()
            .WithTags("Adms");

            endpoint.MapGet("/adms/{id}", ([FromRoute] int id, IAdmService admService) =>
            {
                var adm = admService.FindById(id);
                if (adm == null)
                    return Results.NotFound();
                return Results.Ok(new AdmModelView
                {
                    Id = adm.Id,
                    Email = adm.Email,
                    Perfil = adm.Perfil
                });
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "adm" })
            .WithTags("Adms");

            endpoint.MapGet("/adms", ([FromQuery] int? page, IAdmService admService) =>
            {
                var admsResult = new List<AdmModelView>();
                var adms = admService.All(page);
                adms.ForEach(a => admsResult.Add(new AdmModelView
                {
                    Id = a.Id,
                    Email = a.Email,
                    Perfil = a.Perfil
                }));
                return Results.Ok(admsResult);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "adm" })
            .WithTags("Adms");

            endpoint.MapPost("/adms", ([FromBody] AdmDTO admDTO, IAdmService admService) =>
            {
                var validationResult = ValidateDTO(admDTO);
                if (validationResult != null)
                    return validationResult;

                var adm = new Adm
                {
                    Email = admDTO.Email,
                    Senha = admDTO.Senha,
                    Perfil = admDTO.Perfil.ToString()
                };

                admService.Add(adm);

                var result = new AdmModelView
                {
                    Id = adm.Id,
                    Email = adm.Email,
                    Perfil = adm.Perfil
                };
                return Results.Created($"/adms/{result.Id}", result);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "adm" })
            .WithTags("Adms");
            #endregion

            #region Vehicle
            endpoint.MapPost("/vechiles", ([FromBody] VehicleDTO vehicleDto, IVehicleService vehicleService) =>
            {
                var validationResult = ValidateDTO(vehicleDto);
                if (validationResult != null)
                    return validationResult;

                var vehicle = new Vehicle
                {
                    Nome = vehicleDto.Nome,
                    Marca = vehicleDto.Marca,
                    Ano = vehicleDto.Ano
                };
                vehicleService.Add(vehicle);
                return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "adm,editor" })
            .WithTags("Vehicles");

            endpoint.MapGet("/vechicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
            {
                var vehicles = vehicleService.All(page);
                return Results.Ok(vehicles);
            })
            .RequireAuthorization()
            .WithTags("Vehicles");

            endpoint.MapGet("/vechicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.FindById(id);
                if (vehicle == null)
                    return Results.NotFound();
                return Results.Ok(vehicle);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "adm,editor" })
            .WithTags("Vehicles");

            endpoint.MapPut("/vechicles", ([FromQuery] int id, VehicleDTO vehicleDto, IVehicleService vehicleService) =>
            {
                var validationResult = ValidateDTO(vehicleDto);
                if (validationResult != null)
                    return validationResult;

                var vehicle = vehicleService.FindById(id);
                if (vehicle == null) return Results.NotFound();

                vehicle.Nome = vehicleDto.Nome;
                vehicle.Ano = vehicleDto.Ano;
                vehicle.Marca = vehicleDto.Marca;

                vehicleService.Update(vehicle);
                return Results.Ok(vehicle);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "adm" })
            .WithTags("Vehicles");

            endpoint.MapDelete("/vechicles", ([FromQuery] int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.FindById(id);
                if (vehicle == null) return Results.NotFound();

                vehicleService.Delete(vehicle);
                return Results.NoContent();
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "adm" })
            .WithTags("Vehicles");
            #endregion
        });
    }
}