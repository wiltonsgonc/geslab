using System.ComponentModel.DataAnnotations;

namespace LabScheduler.Domain.Entities
{
    public enum ReservaStatus
    {
        Pendente,
        Confirmada,
        EmAndamento,
        Concluida,
        Cancelada,
        NaoCompareceu
    }

    public class Reserva : BaseEntity
    {
        [Required, MaxLength(200)]
        public string NomeSolicitante { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string EmailSolicitante { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? TelefoneSolicitante { get; set; }

        [MaxLength(300)]
        public string? InstituicaoEmpresa { get; set; }

        public DateTime DataReserva { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFim { get; set; }

        public ReservaStatus Status { get; set; } = ReservaStatus.Pendente;

        public Guid? UsuarioInternoId { get; set; }

        [MaxLength(1000)]
        public string? Observacoes { get; set; }

        public ICollection<ReservaEquipamento> Equipamentos { get; set; } = [];
        public ICollection<ReservaInsumo> Insumos { get; set; } = [];
    }
}
