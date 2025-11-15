namespace minimals_api.Infra.Db;

using Microsoft.EntityFrameworkCore;
using minimals_api.Domain.Entities;

public class DbContexto : DbContext
{
    private readonly IConfiguration _configuracaoAppSettings;
    public DbSet<Adm> Adms { get; set; } = default!;
    public DbSet<Vehicle> vehicles { get; set; } = default!;

    public DbContexto(IConfiguration configuration)
    {
        _configuracaoAppSettings = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adm>().HasData(
        new Adm
        {
            Id = 1,
            Email = "adm@teste.com",
            Senha = "123456",
            Perfil = "Adm"
        }
                );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Only configure if options haven't been provided (e.g. when activated by DI without options)
        if (!optionsBuilder.IsConfigured && _configuracaoAppSettings != null)
        {
            var stringConexao = _configuracaoAppSettings.GetConnectionString("mysql")?.ToString();
            if (!string.IsNullOrEmpty(stringConexao))
            {
                // Avoid AutoDetect here; prefer explicit configuration done in Program.cs. Keep this as a fallback.
                optionsBuilder.UseMySql(
                    stringConexao,
                    ServerVersion.AutoDetect(stringConexao)
                );
            }
        }
    }
}