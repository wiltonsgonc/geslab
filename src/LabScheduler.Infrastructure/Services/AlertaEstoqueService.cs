using LabScheduler.Domain.Entities;
using LabScheduler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabScheduler.Infrastructure.Services
{
    public class AlertaEstoqueService(AppDbContext db)
    {
        private readonly AppDbContext _db = db;

        public async Task<List<AlertaEstoque>> GetAllAsync(AlertaStatus? status = null)
        {
            var query = _db.AlertasEstoque
                .Include(a => a.Insumo)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(a => a.Status == status.Value);

            return await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task ResolverAsync(Guid id, string? acaoTomada, string? responsavel)
        {
            var alerta = await _db.AlertasEstoque.FindAsync(id)
                ?? throw new KeyNotFoundException("Alerta não encontrado.");

            alerta.Status = AlertaStatus.Resolvido;
            alerta.DataResolucao = DateTime.UtcNow;
            alerta.AcaoTomada = acaoTomada;
            alerta.ResponsavelNotificado = responsavel;
            await _db.SaveChangesAsync();
        }

        public async Task<int> GetAlertasAtivosCountAsync()
        {
            return await _db.AlertasEstoque
                .CountAsync(a => a.Status == AlertaStatus.Ativo);
        }
    }
}
