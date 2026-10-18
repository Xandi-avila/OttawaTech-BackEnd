using Microsoft.EntityFrameworkCore;
using Ottawa_Tech___BackEnd.Models;

namespace Ottawa_Tech___BackEnd.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ClienteModel> Clientes => Set<ClienteModel>();
    public DbSet<QuadraModel> Quadras => Set<QuadraModel>();
    public DbSet<ReservaModel> Reservas => Set<ReservaModel>();
    public DbSet<PagamentoModel> Pagamentos => Set<PagamentoModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReservaModel>()
            .HasOne(reserva => reserva.Cliente)
            .WithMany(cliente => cliente.Reservas)
            .HasForeignKey(reserva => reserva.ClienteId);

        modelBuilder.Entity<ReservaModel>()
            .HasOne(reserva => reserva.Quadra)
            .WithMany(quadra => quadra.Reservas)
            .HasForeignKey(reserva => reserva.QuadraId);

        modelBuilder.Entity<ReservaModel>()
            .HasOne(reserva => reserva.Pagamento)
            .WithOne(pagamento => pagamento.Reserva)
            .HasForeignKey<PagamentoModel>(pagamento => pagamento.ReservaId);
    }
}