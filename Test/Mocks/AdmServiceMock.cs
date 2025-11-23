using minimals_api.Domain.DTOs;
using minimals_api.Domain.Entities;
using minimals_api.Domain.Interfaces;
using Org.BouncyCastle.Asn1.Misc;

namespace Test.Mocks;

public class AdmServiceMock : IAdmService
{
    private static List<Adm> adms = new List<Adm>()
    {
        new Adm
        {
            Id = 1,
            Email = "adm@teste.com",
            Senha = "123456",
            Perfil = "Adm"
        },
        new Adm{
            Id = 2,
            Email = "editor@teste.com",
            Senha = "123456",
            Perfil = "Editor"
        }
    };
    public Adm Add(Adm adm)
    {
        adm.Id = adms.Count() + 1;
        adms.Add(adm);

        return adm;
    }

    public List<Adm> All(int? page = 1)
    {
        return adms;
    }

    public Adm? FindById(int id)
    {
        return adms.Find(a => a.Id == id);
    }

    public Adm? Login(LoginDTO loginDTO)
    {
        return adms.Find(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);
    }
}