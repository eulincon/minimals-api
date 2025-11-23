
using System.Net;
using System.Text;
using System.Text.Json;
using minimals_api.Domain.DTOs;
using minimals_api.Domain.ModelViews;
using minimals_api.Migrations;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Crypto.Prng;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class AdmRequestTest
{

    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task GetSetPropsTest()
    {
        var loginDTO = new LoginDTO
        {
            Email = "adm@teste.com",
            Senha = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        var response = await Setup.client.PostAsync("/adms/login", content);

        var result = await response.Content.ReadAsStreamAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var admLogado = JsonSerializer.Deserialize<AdmLogado>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(admLogado);
        Assert.IsNotNull(admLogado.Token);
        Assert.IsNotNull(admLogado.Email);
        Assert.IsNotNull(admLogado.Perfil);
    }
}