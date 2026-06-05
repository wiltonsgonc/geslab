using System.ComponentModel.DataAnnotations;

namespace LabScheduler.Domain.Entities
{
    public enum EquipamentoStatus
    {
        Disponivel,
        EmManutencao,
        Reservado,
        Inativo
    }

    public class Equipamento : BaseEntity
    {
        [Required, MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descricao { get; set; }

        [MaxLength(100)]
        public string? NumeroSerie { get; set; }

        [MaxLength(100)]
        public string? Marca { get; set; }

        [MaxLength(100)]
        public string? Modelo { get; set; }

        public EquipamentoStatus Status { get; set; } = EquipamentoStatus.Disponivel;

        public int? CapacidadeDiaria { get; set; }

        public bool RequerTreinamento { get; set; }

        [MaxLength(500)]
        public string? Observacoes { get; set; }
    }
}
