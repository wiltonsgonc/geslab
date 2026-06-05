namespace LabScheduler.Domain.DTOs
{
    public record DashboardDto(
        int TotalReservasHoje,
        int TotalReservasSemana,
        int EquipamentosDisponiveis,
        int EquipamentosTotal,
        int InsumosEstoqueBaixo,
        int InsumosTotal,
        int ReservasPendentes
    );
}
