
using minimals_api.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class AdmTest
{
    [TestMethod]
    public void GetSetPropsTest()
    {
        var adm = new Adm();
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("teste@teste.com", adm.Email);
        Assert.AreEqual("teste", adm.Senha);
        Assert.AreEqual("Adm", adm.Perfil);
    
    }
}