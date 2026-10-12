using Microsoft.EntityFrameworkCore;
using Ottawa_Tech___BackEnd.Models;

namespace Ottawa_Tech___BackEnd.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Quadra> Quadras => Set<Quadra>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reserva>()
            .HasOne(reserva => reserva.Cliente)
            .WithMany(cliente => cliente.Reservas)
            .HasForeignKey(reserva => reserva.ClienteId);

        modelBuilder.Entity<Reserva>()
            .HasOne(reserva => reserva.Quadra)
            .WithMany(quadra => quadra.Reservas)
            .HasForeignKey(reserva => reserva.QuadraId);

        modelBuilder.Entity<Reserva>()
            .HasOne(reserva => reserva.Pagamento)
            .WithOne(pagamento => pagamento.Reserva)
            .HasForeignKey<Pagamento>(pagamento => pagamento.ReservaId);
    }
}