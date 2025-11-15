using Microsoft.EntityFrameworkCore;
using minimals_api.Domain.DTOs;
using minimals_api.Domain.Entities;
using minimals_api.Domain.Interfaces;
using minimals_api.Infra.Db;

namespace minimals_api.Domain.Services;

public class VehicleService : IVehicleService
{
    private readonly DbContexto _contexto;
    public VehicleService(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public void Add(Vehicle vehicle)
    {
        _contexto.vehicles.Add(vehicle);
        _contexto.SaveChanges();
    }

    public List<Vehicle>? All(int page = 1, string? nome = null, string? marca = null)
    {
        var query = _contexto.vehicles.AsQueryable();
        if (!string.IsNullOrEmpty(nome))
        {
            query.Where(v => v.Nome.ToLower().Contains(nome));
        }
        int itensByPage = 10;
        query = query.Skip((page - 1) * itensByPage).Take(itensByPage);
        return query.ToList();
    }

    public void Delete(Vehicle vehicle)
    {
        _contexto.vehicles.Remove(vehicle);
        _contexto.SaveChanges();
    }

    public Vehicle? FindById(int id)
    {
        return _contexto.vehicles.Where(v => v.Id == id).FirstOrDefault();
    }

    public Adm? Login(LoginDTO loginDTO)
    {
        var adms = _contexto.Adms.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adms;
    }

    public void Update(Vehicle vehicle)
    {
        _contexto.vehicles.Update(vehicle);
        _contexto.SaveChanges();
    }
}