using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OttawaTech.BackEnd.Models;

namespace OttawaTech.BackEnd.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Administrador> Administradores => Set<Administrador>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Quadra> Quadras => Set<Quadra>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>(administrador =>
        {
            administrador.Property(a => a.Nome).HasMaxLength(150);
            administrador.Property(a => a.Email).HasMaxLength(200);
            administrador.HasIndex(a => a.Email).IsUnique();
        });

        modelBuilder.Entity<Cliente>(cliente =>
        {
            cliente.Property(c => c.Nome).HasMaxLength(150);
            cliente.Property(c => c.Cpf).HasMaxLength(14);
            cliente.Property(c => c.Email).HasMaxLength(200);
            cliente.Property(c => c.Telefone).HasMaxLength(20);
            cliente.HasIndex(c => c.Email).IsUnique();
            cliente.HasIndex(c => c.Cpf);
        });

        modelBuilder.Entity<Quadra>(quadra =>
        {
            quadra.Property(q => q.Nome).HasMaxLength(100);
            quadra.Property(q => q.Tipo).HasMaxLength(100);
            quadra.Property(q => q.ValorHora).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Reserva>(reserva =>
        {
            reserva.Property(r => r.Valor).HasPrecision(10, 2);
            reserva.HasIndex(r => new { r.QuadraId, r.Data });
            reserva.Ignore(r => r.Inicio);
            reserva.Ignore(r => r.Fim);
            reserva.Ignore(r => r.PagamentoAtivo);

            reserva.HasOne(r => r.Cliente)
                .WithMany(c => c.Reservas)
                .HasForeignKey(r => r.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            reserva.HasOne(r => r.Quadra)
                .WithMany(q => q.Reservas)
                .HasForeignKey(r => r.QuadraId)
                .OnDelete(DeleteBehavior.Restrict);

            reserva.HasMany(r => r.Pagamentos)
                .WithOne(p => p.Reserva)
                .HasForeignKey(p => p.ReservaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Pagamento>().Property(p => p.Valor).HasPrecision(10, 2);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<DataHoraUtcConverter>();
    }

    private sealed class DataHoraUtcConverter() : ValueConverter<DateTime, DateTime>(
        valor => valor.ToUniversalTime(),
        valor => DateTime.SpecifyKind(valor, DateTimeKind.Utc));
}
