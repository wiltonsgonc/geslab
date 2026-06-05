using System.ComponentModel.DataAnnotations;

namespace LabScheduler.Domain.Entities
{
    public enum AlertaStatus
    {
        Ativo,
        Resolvido,
        Ignorado
    }

    public class AlertaEstoque : BaseEntity
    {
        public Guid InsumoId { get; set; }
        public Insumo Insumo { get; set; } = null!;

        [MaxLength(500)]
        public string Mensagem { get; set; } = string.Empty;

        public decimal QuantidadeAtual { get; set; }

        public decimal QuantidadeMinima { get; set; }

        public AlertaStatus Status { get; set; } = AlertaStatus.Ativo;

        [MaxLength(200)]
        public string? ResponsavelNotificado { get; set; }

        public DateTime? DataResolucao { get; set; }

        [MaxLength(500)]
        public string? AcaoTomada { get; set; }
    }
}
