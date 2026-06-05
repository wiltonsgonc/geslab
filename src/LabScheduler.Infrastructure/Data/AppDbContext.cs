using LabScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabScheduler.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Equipamento> Equipamentos => Set<Equipamento>();
        public DbSet<Insumo> Insumos => Set<Insumo>();
        public DbSet<MovimentoEstoque> MovimentosEstoque => Set<MovimentoEstoque>();
        public DbSet<Reserva> Reservas => Set<Reserva>();
        public DbSet<ReservaEquipamento> ReservaEquipamentos => Set<ReservaEquipamento>();
        public DbSet<ReservaInsumo> ReservaInsumos => Set<ReservaInsumo>();
        public DbSet<AlertaEstoque> AlertasEstoque => Set<AlertaEstoque>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Equipamento>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Nome).IsRequired().HasMaxLength(200);
                e.Property(x => x.Descricao).HasMaxLength(500);
                e.Property(x => x.NumeroSerie).HasMaxLength(100);
                e.Property(x => x.Marca).HasMaxLength(100);
                e.Property(x => x.Modelo).HasMaxLength(100);
                e.Property(x => x.Observacoes).HasMaxLength(500);
                e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
                e.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<Insumo>(i =>
            {
                i.HasKey(x => x.Id);
                i.Property(x => x.Nome).IsRequired().HasMaxLength(200);
                i.Property(x => x.Descricao).HasMaxLength(500);
                i.Property(x => x.CasNumber).HasMaxLength(100);
                i.Property(x => x.Fabricante).HasMaxLength(100);
                i.Property(x => x.Categoria).HasConversion<string>().HasMaxLength(30);
                i.Property(x => x.Unidade).HasConversion<string>().HasMaxLength(20);
                i.Property(x => x.Localizacao).HasMaxLength(500);
                i.Property(x => x.Observacoes).HasMaxLength(500);
                i.Property(x => x.QuantidadeAtual).HasPrecision(18, 4);
                i.Property(x => x.QuantidadeMinima).HasPrecision(18, 4);
                i.Property(x => x.QuantidadeMaxima).HasPrecision(18, 4);
                i.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<MovimentoEstoque>(m =>
            {
                m.HasKey(x => x.Id);
                m.Property(x => x.Tipo).HasConversion<string>().HasMaxLength(20);
                m.Property(x => x.Responsavel).HasMaxLength(200);
                m.Property(x => x.Observacao).HasMaxLength(500);
                m.Property(x => x.Quantidade).HasPrecision(18, 4);
                m.Property(x => x.SaldoAnterior).HasPrecision(18, 4);
                m.Property(x => x.SaldoPosterior).HasPrecision(18, 4);
                m.HasOne(x => x.Insumo).WithMany().HasForeignKey(x => x.InsumoId);
                m.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<Reserva>(r =>
            {
                r.HasKey(x => x.Id);
                r.Property(x => x.NomeSolicitante).IsRequired().HasMaxLength(200);
                r.Property(x => x.EmailSolicitante).IsRequired().HasMaxLength(200);
                r.Property(x => x.TelefoneSolicitante).HasMaxLength(20);
                r.Property(x => x.InstituicaoEmpresa).HasMaxLength(300);
                r.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
                r.Property(x => x.Observacoes).HasMaxLength(1000);
                r.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<ReservaEquipamento>(re =>
            {
                re.HasKey(x => x.Id);
                re.HasOne(x => x.Reserva).WithMany(r => r.Equipamentos).HasForeignKey(x => x.ReservaId);
                re.HasOne(x => x.Equipamento).WithMany().HasForeignKey(x => x.EquipamentoId);
                re.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<ReservaInsumo>(ri =>
            {
                ri.HasKey(x => x.Id);
                ri.HasOne(x => x.Reserva).WithMany(r => r.Insumos).HasForeignKey(x => x.ReservaId);
                ri.HasOne(x => x.Insumo).WithMany().HasForeignKey(x => x.InsumoId);
                ri.Property(x => x.QuantidadePrevista).HasPrecision(18, 4);
                ri.Property(x => x.QuantidadeUtilizada).HasPrecision(18, 4);
                ri.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<AlertaEstoque>(a =>
            {
                a.HasKey(x => x.Id);
                a.Property(x => x.Mensagem).IsRequired().HasMaxLength(500);
                a.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
                a.Property(x => x.ResponsavelNotificado).HasMaxLength(200);
                a.Property(x => x.AcaoTomada).HasMaxLength(500);
                a.Property(x => x.QuantidadeAtual).HasPrecision(18, 4);
                a.Property(x => x.QuantidadeMinima).HasPrecision(18, 4);
                a.HasOne(x => x.Insumo).WithMany().HasForeignKey(x => x.InsumoId);
                a.HasQueryFilter(x => !x.IsDeleted);
            });
        }
    }
}
