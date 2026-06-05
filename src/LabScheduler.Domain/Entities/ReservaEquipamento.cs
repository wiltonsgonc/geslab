namespace LabScheduler.Domain.Entities
{
    public class ReservaEquipamento : BaseEntity
    {
        public Guid ReservaId { get; set; }
        public Reserva Reserva { get; set; } = null!;

        public Guid EquipamentoId { get; set; }
        public Equipamento Equipamento { get; set; } = null!;
    }
}
