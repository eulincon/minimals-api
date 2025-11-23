
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimals_api.Domain.Entities;
using minimals_api.Domain.Services;
using minimals_api.Infra.Db;

namespace Test.Domain.Entities;

[TestClass]
public class AdmServiceTest
{
    private DbContexto ContextTestBuilder()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var options = new ConfigurationBuilder()
        .SetBasePath(path ?? Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

        var configuration = options.Build();

        return new DbContexto(configuration);

    }

    [TestMethod]
    public void AddAdmTest()
    {
        var adm = new Adm();
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";
        var context = ContextTestBuilder();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Adms");

        var admService = new AdmService(context);

        admService.Add(adm);

        Assert.AreEqual(1, admService.All().Count());
    }

    [TestMethod]
    public void FindByIdTest()
    {
        var context = ContextTestBuilder();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Adms");

        var adm = new Adm();
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        var admService = new AdmService(context);

        admService.Add(adm);
        var admDatabase = admService.FindById(adm.Id);

        Assert.AreEqual(1, admDatabase?.Id);
    }
}