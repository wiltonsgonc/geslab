namespace LabScheduler.Web.Models
{
    public class MovimentoEstoque
    {
        public Guid Id { get; set; }
        public Guid InsumoId { get; set; }
        public string NomeInsumo { get; set; } = "";
        public string Tipo { get; set; } = "";
        public decimal Quantidade { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoPosterior { get; set; }
        public string? Responsavel { get; set; }
        public string? Observacao { get; set; }
        public DateTime DataMovimento { get; set; }
    }

    public class CreateMovimento
    {
        public Guid InsumoId { get; set; }
        public string Tipo { get; set; } = "";
        public decimal Quantidade { get; set; }
        public string? Responsavel { get; set; }
        public string? Observacao { get; set; }
    }
}
