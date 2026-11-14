using Microsoft.EntityFrameworkCore;
using Vendinha.WinForms.Models;

namespace Vendinha.WinForms.Data;

public sealed class VendinhaDbContext : DbContext
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Divida> Dividas => Set<Divida>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string path = DatabaseLocation.GetDatabasePath();
            optionsBuilder.UseSqlite($"Data Source={path}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.NomeCompleto).IsRequired().HasMaxLength(150);
            entity.Property(c => c.CPF).IsRequired().HasMaxLength(11);
            entity.Property(c => c.Email);
            entity.Property(c => c.DataNascimento).IsRequired();
            entity.HasIndex(c => c.CPF).IsUnique();
        });

        modelBuilder.Entity<Divida>(entity =>
        {
            entity.ToTable("Dividas");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Valor).HasColumnType("decimal(10,2)");
            entity.Property(d => d.ValorPago).HasColumnType("decimal(10,2)").HasDefaultValue(0m);
            entity.Property(d => d.Paga).IsRequired();
            entity.Property(d => d.DataCriacao).IsRequired();
            entity.HasIndex(d => d.ClienteId);
            entity.HasOne(d => d.Cliente)
                .WithMany()
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });
    }
}
