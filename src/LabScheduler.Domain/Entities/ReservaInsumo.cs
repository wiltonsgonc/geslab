using System.ComponentModel.DataAnnotations;

namespace LabScheduler.Domain.Entities
{
    public class ReservaInsumo : BaseEntity
    {
        public Guid ReservaId { get; set; }
        public Reserva Reserva { get; set; } = null!;

        public Guid InsumoId { get; set; }
        public Insumo Insumo { get; set; } = null!;

        public decimal QuantidadePrevista { get; set; }

        public decimal? QuantidadeUtilizada { get; set; }
    }
}
