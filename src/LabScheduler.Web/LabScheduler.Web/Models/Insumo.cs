namespace LabScheduler.Web.Models
{
    public class Insumo
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = "";
        public string? Descricao { get; set; }
        public string? CasNumber { get; set; }
        public string? Fabricante { get; set; }
        public string Categoria { get; set; } = "Outro";
        public string Unidade { get; set; } = "Unidade";
        public decimal QuantidadeAtual { get; set; }
        public decimal QuantidadeMinima { get; set; }
        public decimal? QuantidadeMaxima { get; set; }
        public string? Localizacao { get; set; }
        public bool Controlado { get; set; }
        public bool EstoqueBaixo { get; set; }
        public string? Observacoes { get; set; }
    }

    public class CreateInsumo
    {
        public string Nome { get; set; } = "";
        public string? Descricao { get; set; }
        public string? CasNumber { get; set; }
        public string? Fabricante { get; set; }
        public string Categoria { get; set; } = "Outro";
        public string Unidade { get; set; } = "Unidade";
        public decimal QuantidadeAtual { get; set; }
        public decimal QuantidadeMinima { get; set; }
        public decimal? QuantidadeMaxima { get; set; }
        public string? Localizacao { get; set; }
        public bool Controlado { get; set; }
        public string? Observacoes { get; set; }
    }

    public class UpdateInsumo
    {
        public string Nome { get; set; } = "";
        public string? Descricao { get; set; }
        public string? CasNumber { get; set; }
        public string? Fabricante { get; set; }
        public string Categoria { get; set; } = "Outro";
        public string Unidade { get; set; } = "Unidade";
        public decimal QuantidadeAtual { get; set; }
        public decimal QuantidadeMinima { get; set; }
        public decimal? QuantidadeMaxima { get; set; }
        public string? Localizacao { get; set; }
        public bool Controlado { get; set; }
        public string? Observacoes { get; set; }
    }
}
