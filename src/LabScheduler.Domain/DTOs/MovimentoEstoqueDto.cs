using LabScheduler.Domain.Entities;

namespace LabScheduler.Domain.DTOs
{
    public record MovimentoEstoqueDto(
        Guid Id,
        Guid InsumoId,
        string NomeInsumo,
        TipoMovimento Tipo,
        decimal Quantidade,
        decimal SaldoAnterior,
        decimal SaldoPosterior,
        string? Responsavel,
        string? Observacao,
        DateTime DataMovimento
    );

    public record CreateMovimentoDto(
        Guid InsumoId,
        TipoMovimento Tipo,
        decimal Quantidade,
        string? Responsavel,
        string? Observacao
    );
}
