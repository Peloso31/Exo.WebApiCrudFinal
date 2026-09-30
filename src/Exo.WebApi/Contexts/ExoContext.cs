using Exo.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Exo.WebApi.Contexts;

/// <summary>
/// EF Core context. The database provider and connection string are
/// configured in Program.cs, so this class has no hard-coded settings.
/// </summary>
public class ExoContext : DbContext
{
    public ExoContext(DbContextOptions<ExoContext> options) : base(options)
    {
    }

    public DbSet<Projeto> Projetos => Set<Projeto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Projeto>(entity =>
        {
            entity.ToTable("Projetos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.NomeDoProjeto).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Area).IsRequired().HasMaxLength(50);
        });
    }
}
