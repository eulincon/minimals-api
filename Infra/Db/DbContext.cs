namespace minimals_api.Infra.Db;

using Microsoft.EntityFrameworkCore;
using minimals_api.Domain.Entities;

public class DbContexto : DbContext
{
    private readonly IConfiguration _configuration;
    public DbContexto(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbSet<Adm> Adms { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // base.OnConfiguring(optionsBuilder);
            var stringConexao = _configuration.GetConnectionString("mysql")?.ToString();
            if (string.IsNullOrEmpty(stringConexao))
            {
                optionsBuilder.UseMySql(stringConexao,
                ServerVersion.AutoDetect(stringConexao));
            }
        }
    }
}