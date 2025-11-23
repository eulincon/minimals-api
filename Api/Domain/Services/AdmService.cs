using minimals_api.Domain.DTOs;
using minimals_api.Domain.Entities;
using minimals_api.Domain.Interfaces;
using minimals_api.Infra.Db;

namespace minimals_api.Domain.Services;

public class AdmService : IAdmService
{
    private readonly DbContexto _contexto;
    public AdmService(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public Adm Add(Adm adm)
    {
        _contexto.Adms.Add(adm);
        _contexto.SaveChanges();
        return adm;
    }

    public Adm? Login(LoginDTO loginDTO)
    {
        var adms = _contexto.Adms.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adms;
    }

    public List<Adm> All(int? page = 1)
    {
        var query = _contexto.Adms.AsQueryable();
        int itensByPage = 10;
        query = query.Skip((page.GetValueOrDefault(1) - 1) * itensByPage).Take(itensByPage);
        return query.ToList();
    }

    public Adm? FindById(int id)
    {
        return _contexto.Adms.Where(a => a.Id == id).FirstOrDefault();
    }
}