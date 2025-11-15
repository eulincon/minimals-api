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
    public Adm? Login(LoginDTO loginDTO)
    {
        var adms = _contexto.Adms.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adms;
    }
}