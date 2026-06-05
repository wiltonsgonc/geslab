using LabScheduler.Domain.Entities;

namespace LabScheduler.Domain.DTOs
{
    public record EquipamentoDto(
        Guid Id,
        string Nome,
        string? Descricao,
        string? NumeroSerie,
        string? Marca,
        string? Modelo,
        EquipamentoStatus Status,
        int? CapacidadeDiaria,
        bool RequerTreinamento,
        string? Observacoes
    );

    public record CreateEquipamentoDto(
        string Nome,
        string? Descricao,
        string? NumeroSerie,
        string? Marca,
        string? Modelo,
        int? CapacidadeDiaria,
        bool RequerTreinamento,
        string? Observacoes
    );

    public record UpdateEquipamentoDto(
        string Nome,
        string? Descricao,
        string? NumeroSerie,
        string? Marca,
        string? Modelo,
        EquipamentoStatus Status,
        int? CapacidadeDiaria,
        bool RequerTreinamento,
        string? Observacoes
    );
}
