namespace LabScheduler.Web.Models
{
    public class Reserva
    {
        public Guid Id { get; set; }
        public string NomeSolicitante { get; set; } = "";
        public string EmailSolicitante { get; set; } = "";
        public string? TelefoneSolicitante { get; set; }
        public string? InstituicaoEmpresa { get; set; }
        public DateTime DataReserva { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public string Status { get; set; } = "Pendente";
        public string? Observacoes { get; set; }
        public List<EquipamentoReservado> Equipamentos { get; set; } = [];
        public List<InsumoReservado> Insumos { get; set; } = [];
        public DateTime CreatedAt { get; set; }
    }

    public class EquipamentoReservado
    {
        public Guid Id { get; set; }
        public Guid EquipamentoId { get; set; }
        public string NomeEquipamento { get; set; } = "";
    }

    public class InsumoReservado
    {
        public Guid Id { get; set; }
        public Guid InsumoId { get; set; }
        public string NomeInsumo { get; set; } = "";
        public decimal QuantidadePrevista { get; set; }
        public decimal? QuantidadeUtilizada { get; set; }
    }

    public class CreateReserva
    {
        public string NomeSolicitante { get; set; } = "";
        public string EmailSolicitante { get; set; } = "";
        public string? TelefoneSolicitante { get; set; }
        public string? InstituicaoEmpresa { get; set; }
        public DateTime DataReserva { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public string? Observacoes { get; set; }
        public List<Guid> EquipamentoIds { get; set; } = [];
        public List<ReservaInsumoItem> Insumos { get; set; } = [];
    }

    public class ReservaInsumoItem
    {
        public Guid InsumoId { get; set; }
        public decimal QuantidadePrevista { get; set; }
    }

    public class ReservaCalendario
    {
        public Guid Id { get; set; }
        public DateTime DataReserva { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public string NomeSolicitante { get; set; } = "";
        public string InstituicaoEmpresa { get; set; } = "";
        public string Status { get; set; } = "";
        public List<string> Equipamentos { get; set; } = [];
    }
}
