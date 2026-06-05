namespace LabScheduler.Web.Models
{
    public class AlertaEstoque
    {
        public Guid Id { get; set; }
        public Guid InsumoId { get; set; }
        public string NomeInsumo { get; set; } = "";
        public string Mensagem { get; set; } = "";
        public decimal QuantidadeAtual { get; set; }
        public decimal QuantidadeMinima { get; set; }
        public string Status { get; set; } = "Ativo";
        public string? ResponsavelNotificado { get; set; }
        public DateTime? DataResolucao { get; set; }
        public string? AcaoTomada { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Dashboard
    {
        public int TotalReservasHoje { get; set; }
        public int TotalReservasSemana { get; set; }
        public int EquipamentosDisponiveis { get; set; }
        public int EquipamentosTotal { get; set; }
        public int InsumosEstoqueBaixo { get; set; }
        public int InsumosTotal { get; set; }
        public int ReservasPendentes { get; set; }
    }
}
