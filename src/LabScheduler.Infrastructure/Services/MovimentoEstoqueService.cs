using LabScheduler.Domain.DTOs;
using LabScheduler.Domain.Entities;
using LabScheduler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabScheduler.Infrastructure.Services
{
    public class MovimentoEstoqueService(AppDbContext db)
    {
        private readonly AppDbContext _db = db;

        public async Task<List<MovimentoEstoqueDto>> GetAllAsync(Guid? insumoId = null)
        {
            var query = _db.MovimentosEstoque
                .Include(m => m.Insumo)
                .AsQueryable();

            if (insumoId.HasValue)
                query = query.Where(m => m.InsumoId == insumoId.Value);

            return await query
                .OrderByDescending(m => m.DataMovimento)
                .Select(m => ToDto(m))
                .ToListAsync();
        }

        public async Task<MovimentoEstoqueDto> CreateAsync(CreateMovimentoDto dto)
        {
            var insumo = await _db.Insumos.FindAsync(dto.InsumoId)
                ?? throw new KeyNotFoundException("Insumo não encontrado.");

            var saldoAnterior = insumo.QuantidadeAtual;

            if (dto.Tipo == TipoMovimento.Saida || dto.Tipo == TipoMovimento.Reserva)
            {
                if (insumo.QuantidadeAtual < dto.Quantidade)
                    throw new InvalidOperationException("Estoque insuficiente.");
                insumo.QuantidadeAtual -= dto.Quantidade;
            }
            else if (dto.Tipo == TipoMovimento.Entrada || dto.Tipo == TipoMovimento.Devolucao)
            {
                insumo.QuantidadeAtual += dto.Quantidade;
            }
            else
            {
                insumo.QuantidadeAtual = dto.Quantidade;
            }

            var movimento = new MovimentoEstoque
            {
                InsumoId = dto.InsumoId,
                Tipo = dto.Tipo,
                Quantidade = dto.Quantidade,
                SaldoAnterior = saldoAnterior,
                SaldoPosterior = insumo.QuantidadeAtual,
                Responsavel = dto.Responsavel,
                Observacao = dto.Observacao
            };

            _db.MovimentosEstoque.Add(movimento);
            insumo.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

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

            return ToDto(movimento);
        }

        private static MovimentoEstoqueDto ToDto(MovimentoEstoque m) => new(
            m.Id, m.InsumoId, m.Insumo?.Nome ?? "",
            m.Tipo, m.Quantidade, m.SaldoAnterior, m.SaldoPosterior,
            m.Responsavel, m.Observacao, m.DataMovimento
        );
    }
}
