using LabScheduler.Domain.Entities;

namespace LabScheduler.Domain.DTOs
{
    public record InsumoDto(
        Guid Id,
        string Nome,
        string? Descricao,
        string? CasNumber,
        string? Fabricante,
        InsumoCategoria Categoria,
        UnidadeMedida Unidade,
        decimal QuantidadeAtual,
        decimal QuantidadeMinima,
        decimal? QuantidadeMaxima,
        string? Localizacao,
        bool Controlado,
        bool EstoqueBaixo,
        string? Observacoes
    );

    public record CreateInsumoDto(
        string Nome,
        string? Descricao,
        string? CasNumber,
        string? Fabricante,
        InsumoCategoria Categoria,
        UnidadeMedida Unidade,
        decimal QuantidadeAtual,
        decimal QuantidadeMinima,
        decimal? QuantidadeMaxima,
        string? Localizacao,
        bool Controlado,
        string? Observacoes
    );

    public record UpdateInsumoDto(
        string Nome,
        string? Descricao,
        string? CasNumber,
        string? Fabricante,
        InsumoCategoria Categoria,
        UnidadeMedida Unidade,
        decimal QuantidadeAtual,
        decimal QuantidadeMinima,
        decimal? QuantidadeMaxima,
        string? Localizacao,
        bool Controlado,
        string? Observacoes
    );
}
