using System.ComponentModel.DataAnnotations;

namespace LabScheduler.Domain.Entities
{
    public enum InsumoCategoria
    {
        Reagente,
        Solvente,
        Vidraria,
        EquipamentoProtecao,
        PadraoAnalitico,
        Consumivel,
        Outro
    }

    public enum UnidadeMedida
    {
        Unidade,
        Mililitro,
        Litro,
        Grama,
        Miligrama,
        Quilograma,
        Mol,
        Percentual
    }

    public class Insumo : BaseEntity
    {
        [Required, MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descricao { get; set; }

        [MaxLength(100)]
        public string? CasNumber { get; set; }

        [MaxLength(100)]
        public string? Fabricante { get; set; }

        public InsumoCategoria Categoria { get; set; } = InsumoCategoria.Outro;

        public UnidadeMedida Unidade { get; set; } = UnidadeMedida.Unidade;

        public decimal QuantidadeAtual { get; set; }

        public decimal QuantidadeMinima { get; set; }

        public decimal? QuantidadeMaxima { get; set; }

        [MaxLength(500)]
        public string? Localizacao { get; set; }

        public bool Controlado { get; set; }

        public bool EstoqueBaixo => QuantidadeAtual <= QuantidadeMinima;

        [MaxLength(500)]
        public string? Observacoes { get; set; }
    }
}
