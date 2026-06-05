using LabScheduler.Domain.Entities;

namespace LabScheduler.Domain.DTOs
{
    public record ReservaDto(
        Guid Id,
        string NomeSolicitante,
        string EmailSolicitante,
        string? TelefoneSolicitante,
        string? InstituicaoEmpresa,
        DateTime DataReserva,
        TimeSpan HoraInicio,
        TimeSpan HoraFim,
        ReservaStatus Status,
        string? Observacoes,
        List<EquipamentoReservadoDto> Equipamentos,
        List<InsumoReservadoDto> Insumos,
        DateTime CreatedAt
    );

    public record EquipamentoReservadoDto(Guid Id, Guid EquipamentoId, string NomeEquipamento);

    public record InsumoReservadoDto(Guid Id, Guid InsumoId, string NomeInsumo, decimal QuantidadePrevista, decimal? QuantidadeUtilizada);

    public record CreateReservaDto(
        string NomeSolicitante,
        string EmailSolicitante,
        string? TelefoneSolicitante,
        string? InstituicaoEmpresa,
        DateTime DataReserva,
        TimeSpan HoraInicio,
        TimeSpan HoraFim,
        string? Observacoes,
        List<Guid> EquipamentoIds,
        List<ReservaInsumoDto> Insumos
    );

    public record ReservaInsumoDto(Guid InsumoId, decimal QuantidadePrevista);

    public record ReservaCalendarioDto(
        Guid Id,
        DateTime DataReserva,
        TimeSpan HoraInicio,
        TimeSpan HoraFim,
        string NomeSolicitante,
        string InstituicaoEmpresa,
        ReservaStatus Status,
        List<string> Equipamentos
    );

    public record UpdateReservaStatusDto(ReservaStatus Status);
}
