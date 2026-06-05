namespace LabScheduler.Web.Models
{
    public class Equipamento
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = "";
        public string? Descricao { get; set; }
        public string? NumeroSerie { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string Status { get; set; } = "Disponivel";
        public int? CapacidadeDiaria { get; set; }
        public bool RequerTreinamento { get; set; }
        public string? Observacoes { get; set; }
    }

    public class CreateEquipamento
    {
        public string Nome { get; set; } = "";
        public string? Descricao { get; set; }
        public string? NumeroSerie { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int? CapacidadeDiaria { get; set; }
        public bool RequerTreinamento { get; set; }
        public string? Observacoes { get; set; }
    }

    public class UpdateEquipamento
    {
        public string Nome { get; set; } = "";
        public string? Descricao { get; set; }
        public string? NumeroSerie { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string Status { get; set; } = "Disponivel";
        public int? CapacidadeDiaria { get; set; }
        public bool RequerTreinamento { get; set; }
        public string? Observacoes { get; set; }
    }
}
