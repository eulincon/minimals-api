using minimals_api.Domain.DTOs;
using minimals_api.Domain.Entities;

namespace minimals_api.Domain.Interfaces;

public interface IAdmService
{
    Adm? Login(LoginDTO loginDTO);
}