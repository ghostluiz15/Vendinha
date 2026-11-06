
using Microsoft.EntityFrameworkCore;
using Vendinha.Models;

namespace Vendinha.Contexts
{
    public class VendinhaDbContext : DbContext
    {
        public VendinhaDbContext(DbContextOptions<VendinhaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Divida> Dividas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Um cliente pode possuir várias dívidas
            modelBuilder.Entity<Divida>()
                .HasOne(d => d.Cliente)
                .WithMany()
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            // CPF não pode ser duplicado
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.CPF)
                .IsUnique();
        }
    }
}
