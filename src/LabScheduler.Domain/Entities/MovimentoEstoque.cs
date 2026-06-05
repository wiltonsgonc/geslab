using System.ComponentModel.DataAnnotations;

namespace LabScheduler.Domain.Entities
{
    public enum TipoMovimento
    {
        Entrada,
        Saida,
        Ajuste,
        Reserva,
        Devolucao,
        Perda
    }

    public class MovimentoEstoque : BaseEntity
    {
        public Guid InsumoId { get; set; }
        public Insumo Insumo { get; set; } = null!;

        public TipoMovimento Tipo { get; set; }

        public decimal Quantidade { get; set; }

        public decimal SaldoAnterior { get; set; }

        public decimal SaldoPosterior { get; set; }

        [MaxLength(200)]
        public string? Responsavel { get; set; }

        public Guid? ReservaId { get; set; }

        [MaxLength(500)]
        public string? Observacao { get; set; }

        public DateTime DataMovimento { get; set; } = DateTime.UtcNow;
    }
}
