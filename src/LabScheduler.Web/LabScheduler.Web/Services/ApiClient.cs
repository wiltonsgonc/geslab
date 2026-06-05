using LabScheduler.Web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LabScheduler.Web.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private string? _token;

        public ApiClient(HttpClient http) => _http = http;

        public void SetAuthToken(string token)
        {
            _token = token;
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearAuthToken()
        {
            _token = null;
            _http.DefaultRequestHeaders.Authorization = null;
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url, object? body = null)
        {
            var request = new HttpRequestMessage(method, url);
            if (body != null)
                request.Content = JsonContent.Create(body);
            return request;
        }

        // Equipamentos
        public Task<List<Equipamento>> GetEquipamentosAsync() =>
            _http.GetFromJsonAsync<List<Equipamento>>("api/equipamentos") ?? Task.FromResult(new List<Equipamento>());

        public Task<Equipamento?> GetEquipamentoAsync(Guid id) =>
            _http.GetFromJsonAsync<Equipamento>($"api/equipamentos/{id}");

        public async Task<Equipamento> CreateEquipamentoAsync(CreateEquipamento dto)
        {
            var response = await _http.PostAsJsonAsync("api/equipamentos", dto);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Equipamento>())!;
        }

        public async Task UpdateEquipamentoAsync(Guid id, UpdateEquipamento dto)
        {
            var response = await _http.PutAsJsonAsync($"api/equipamentos/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteEquipamentoAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"api/equipamentos/{id}");
            response.EnsureSuccessStatusCode();
        }

        // Insumos
        public Task<List<Insumo>> GetInsumosAsync() =>
            _http.GetFromJsonAsync<List<Insumo>>("api/insumos") ?? Task.FromResult(new List<Insumo>());

        public Task<Insumo?> GetInsumoAsync(Guid id) =>
            _http.GetFromJsonAsync<Insumo>($"api/insumos/{id}");

        public Task<List<Insumo>> GetInsumosEstoqueBaixoAsync() =>
            _http.GetFromJsonAsync<List<Insumo>>("api/insumos/estoque-baixo") ?? Task.FromResult(new List<Insumo>());

        public async Task<Insumo> CreateInsumoAsync(CreateInsumo dto)
        {
            var response = await _http.PostAsJsonAsync("api/insumos", dto);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Insumo>())!;
        }

        public async Task UpdateInsumoAsync(Guid id, UpdateInsumo dto)
        {
            var response = await _http.PutAsJsonAsync($"api/insumos/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteInsumoAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"api/insumos/{id}");
            response.EnsureSuccessStatusCode();
        }

        // Movimentos
        public Task<List<MovimentoEstoque>> GetMovimentosAsync(Guid? insumoId = null)
        {
            var url = insumoId.HasValue ? $"api/movimentos-estoque?insumoId={insumoId}" : "api/movimentos-estoque";
            return _http.GetFromJsonAsync<List<MovimentoEstoque>>(url) ?? Task.FromResult(new List<MovimentoEstoque>());
        }

        public async Task<MovimentoEstoque> CreateMovimentoAsync(CreateMovimento dto)
        {
            var response = await _http.PostAsJsonAsync("api/movimentos-estoque", dto);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<MovimentoEstoque>())!;
        }

        // Reservas
        public Task<List<Reserva>> GetReservasAsync(DateTime? data = null)
        {
            var url = data.HasValue ? $"api/reservas?data={data:yyyy-MM-dd}" : "api/reservas";
            return _http.GetFromJsonAsync<List<Reserva>>(url) ?? Task.FromResult(new List<Reserva>());
        }

        public Task<Reserva?> GetReservaAsync(Guid id) =>
            _http.GetFromJsonAsync<Reserva>($"api/reservas/{id}");

        public Task<List<ReservaCalendario>> GetCalendarioAsync(DateTime inicio, DateTime fim) =>
            _http.GetFromJsonAsync<List<ReservaCalendario>>($"api/reservas/calendario?inicio={inicio:yyyy-MM-dd}&fim={fim:yyyy-MM-dd}")
            ?? Task.FromResult(new List<ReservaCalendario>());

        public Task<List<TimeSpan>> GetHorariosDisponiveisAsync(DateTime data, Guid? equipamentoId = null)
        {
            var url = $"api/reservas/horarios-disponiveis?data={data:yyyy-MM-dd}";
            if (equipamentoId.HasValue) url += $"&equipamentoId={equipamentoId}";
            return _http.GetFromJsonAsync<List<TimeSpan>>(url) ?? Task.FromResult(new List<TimeSpan>());
        }

        public async Task<Reserva> CreateReservaAsync(CreateReserva dto)
        {
            var response = await _http.PostAsJsonAsync("api/reservas", dto);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Reserva>())!;
        }

        public async Task UpdateReservaStatusAsync(Guid id, string status)
        {
            var response = await _http.PatchAsJsonAsync($"api/reservas/{id}/status", new { status });
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteReservaAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"api/reservas/{id}");
            response.EnsureSuccessStatusCode();
        }

        // Dashboard
        public Task<Dashboard> GetDashboardAsync() =>
            _http.GetFromJsonAsync<Dashboard>("api/reservas/dashboard") ?? Task.FromResult(new Dashboard());

        // Alertas
        public Task<List<AlertaEstoque>> GetAlertasAsync(string? status = null)
        {
            var url = status != null ? $"api/alertas?status={status}" : "api/alertas";
            return _http.GetFromJsonAsync<List<AlertaEstoque>>(url) ?? Task.FromResult(new List<AlertaEstoque>());
        }

        public async Task ResolverAlertaAsync(Guid id, string? acao, string? responsavel)
        {
            var response = await _http.PostAsJsonAsync($"api/alertas/{id}/resolver", new { acaoTomada = acao, responsavel });
            response.EnsureSuccessStatusCode();
        }

        public Task<int> GetAlertasAtivosCountAsync() =>
            _http.GetFromJsonAsync<int>("api/alertas/ativos/count") ?? Task.FromResult(0);
    }
}
