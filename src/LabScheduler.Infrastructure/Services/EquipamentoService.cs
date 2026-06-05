using LabScheduler.Domain.DTOs;
using LabScheduler.Domain.Entities;
using LabScheduler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabScheduler.Infrastructure.Services
{
    public class EquipamentoService(AppDbContext db)
    {
        private readonly AppDbContext _db = db;

        public async Task<List<EquipamentoDto>> GetAllAsync()
        {
            return await _db.Equipamentos
                .OrderBy(e => e.Nome)
                .Select(e => ToDto(e))
                .ToListAsync();
        }

        public async Task<EquipamentoDto?> GetByIdAsync(Guid id)
        {
            var e = await _db.Equipamentos.FindAsync(id);
            return e is null ? null : ToDto(e);
        }

        public async Task<EquipamentoDto> CreateAsync(CreateEquipamentoDto dto)
        {
            var entity = new Equipamento
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                NumeroSerie = dto.NumeroSerie,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                CapacidadeDiaria = dto.CapacidadeDiaria,
                RequerTreinamento = dto.RequerTreinamento,
                Observacoes = dto.Observacoes
            };
            _db.Equipamentos.Add(entity);
            await _db.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<EquipamentoDto?> UpdateAsync(Guid id, UpdateEquipamentoDto dto)
        {
            var e = await _db.Equipamentos.FindAsync(id);
            if (e is null) return null;

            e.Nome = dto.Nome;
            e.Descricao = dto.Descricao;
            e.NumeroSerie = dto.NumeroSerie;
            e.Marca = dto.Marca;
            e.Modelo = dto.Modelo;
            e.Status = dto.Status;
            e.CapacidadeDiaria = dto.CapacidadeDiaria;
            e.RequerTreinamento = dto.RequerTreinamento;
            e.Observacoes = dto.Observacoes;
            e.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return ToDto(e);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var e = await _db.Equipamentos.FindAsync(id);
            if (e is null) return false;

            e.IsDeleted = true;
            e.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        private static EquipamentoDto ToDto(Equipamento e) => new(
            e.Id, e.Nome, e.Descricao, e.NumeroSerie, e.Marca, e.Modelo,
            e.Status, e.CapacidadeDiaria, e.RequerTreinamento, e.Observacoes
        );
    }
}
