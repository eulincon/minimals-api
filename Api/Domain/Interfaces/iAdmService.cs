using minimals_api.Domain.DTOs;
using minimals_api.Domain.Entities;

namespace minimals_api.Domain.Interfaces;

public interface IAdmService
{
    Adm? Login(LoginDTO loginDTO);
    Adm Add(Adm adm);
    List<Adm> All(int? page = 1);
    Adm? FindById(int id);
}