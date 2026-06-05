using LabScheduler.Domain.DTOs;
using LabScheduler.Domain.Entities;
using LabScheduler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabScheduler.Infrastructure.Services
{
    public class InsumoService(AppDbContext db)
    {
        private readonly AppDbContext _db = db;

        public async Task<List<InsumoDto>> GetAllAsync()
        {
            return await _db.Insumos
                .OrderBy(i => i.Nome)
                .Select(i => ToDto(i))
                .ToListAsync();
        }

        public async Task<InsumoDto?> GetByIdAsync(Guid id)
        {
            var i = await _db.Insumos.FindAsync(id);
            return i is null ? null : ToDto(i);
        }

        public async Task<InsumoDto> CreateAsync(CreateInsumoDto dto)
        {
            var entity = new Insumo
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                CasNumber = dto.CasNumber,
                Fabricante = dto.Fabricante,
                Categoria = dto.Categoria,
                Unidade = dto.Unidade,
                QuantidadeAtual = dto.QuantidadeAtual,
                QuantidadeMinima = dto.QuantidadeMinima,
                QuantidadeMaxima = dto.QuantidadeMaxima,
                Localizacao = dto.Localizacao,
                Controlado = dto.Controlado,
                Observacoes = dto.Observacoes
            };
            _db.Insumos.Add(entity);
            await _db.SaveChangesAsync();

            await VerificarAlertaEstoque(entity);

            return ToDto(entity);
        }

        public async Task<InsumoDto?> UpdateAsync(Guid id, UpdateInsumoDto dto)
        {
            var i = await _db.Insumos.FindAsync(id);
            if (i is null) return null;

            i.Nome = dto.Nome;
            i.Descricao = dto.Descricao;
            i.CasNumber = dto.CasNumber;
            i.Fabricante = dto.Fabricante;
            i.Categoria = dto.Categoria;
            i.Unidade = dto.Unidade;
            i.QuantidadeAtual = dto.QuantidadeAtual;
            i.QuantidadeMinima = dto.QuantidadeMinima;
            i.QuantidadeMaxima = dto.QuantidadeMaxima;
            i.Localizacao = dto.Localizacao;
            i.Controlado = dto.Controlado;
            i.Observacoes = dto.Observacoes;
            i.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            await VerificarAlertaEstoque(i);

            return ToDto(i);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var i = await _db.Insumos.FindAsync(id);
            if (i is null) return false;

            i.IsDeleted = true;
            i.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<InsumoDto>> GetEstoqueBaixoAsync()
        {
            return await _db.Insumos
                .Where(i => i.QuantidadeAtual <= i.QuantidadeMinima)
                .OrderBy(i => i.Nome)
                .Select(i => ToDto(i))
                .ToListAsync();
        }

        private async Task VerificarAlertaEstoque(Insumo insumo)
        {
            if (insumo.QuantidadeAtual <= insumo.QuantidadeMinima)
            {
                var alertaAtivo = await _db.AlertasEstoque
                    .AnyAsync(a => a.InsumoId == insumo.Id && a.Status == AlertaStatus.Ativo);

                if (!alertaAtivo)
                {
                    _db.AlertasEstoque.Add(new AlertaEstoque
                    {
                        InsumoId = insumo.Id,
                        Mensagem = $"Estoque baixo: {insumo.Nome}. Atual: {insumo.QuantidadeAtual} {insumo.Unidade}, Mínimo: {insumo.QuantidadeMinima} {insumo.Unidade}",
                        QuantidadeAtual = insumo.QuantidadeAtual,
                        QuantidadeMinima = insumo.QuantidadeMinima,
                        Status = AlertaStatus.Ativo
                    });
                    await _db.SaveChangesAsync();
                }
            }
        }

        private static InsumoDto ToDto(Insumo i) => new(
            i.Id, i.Nome, i.Descricao, i.CasNumber, i.Fabricante,
            i.Categoria, i.Unidade, i.QuantidadeAtual, i.QuantidadeMinima,
            i.QuantidadeMaxima, i.Localizacao, i.Controlado, i.EstoqueBaixo, i.Observacoes
        );
    }
}
