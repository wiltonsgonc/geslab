using LabScheduler.Domain.DTOs;
using LabScheduler.Domain.Entities;
using LabScheduler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabScheduler.Infrastructure.Services
{
    public class ReservaService(AppDbContext db)
    {
        private readonly AppDbContext _db = db;

        public async Task<List<ReservaDto>> GetAllAsync(DateTime? data = null)
        {
            var query = _db.Reservas
                .Include(r => r.Equipamentos).ThenInclude(re => re.Equipamento)
                .Include(r => r.Insumos).ThenInclude(ri => ri.Insumo)
                .AsQueryable();

            if (data.HasValue)
                query = query.Where(r => r.DataReserva.Date == data.Value.Date);

            return await query
                .OrderByDescending(r => r.DataReserva)
                .ThenBy(r => r.HoraInicio)
                .Select(r => ToDto(r))
                .ToListAsync();
        }

        public async Task<ReservaDto?> GetByIdAsync(Guid id)
        {
            var r = await _db.Reservas
                .Include(x => x.Equipamentos).ThenInclude(re => re.Equipamento)
                .Include(x => x.Insumos).ThenInclude(ri => ri.Insumo)
                .FirstOrDefaultAsync(x => x.Id == id);

            return r is null ? null : ToDto(r);
        }

        public async Task<List<ReservaCalendarioDto>> GetCalendarioAsync(DateTime inicio, DateTime fim)
        {
            return await _db.Reservas
                .Include(r => r.Equipamentos).ThenInclude(re => re.Equipamento)
                .Where(r => r.DataReserva >= inicio && r.DataReserva <= fim
                    && r.Status != ReservaStatus.Cancelada)
                .OrderBy(r => r.DataReserva)
                .ThenBy(r => r.HoraInicio)
                .Select(r => new ReservaCalendarioDto(
                    r.Id,
                    r.DataReserva,
                    r.HoraInicio,
                    r.HoraFim,
                    r.NomeSolicitante,
                    r.InstituicaoEmpresa ?? "",
                    r.Status,
                    r.Equipamentos.Select(e => e.Equipamento.Nome).ToList()
                ))
                .ToListAsync();
        }

        public async Task<List<TimeSpan>> GetHorariosDisponiveisAsync(DateTime data, Guid? equipamentoId = null)
        {
            var horariosOcupados = await _db.Reservas
                .Include(r => r.Equipamentos)
                .Where(r => r.DataReserva.Date == data.Date
                    && r.Status != ReservaStatus.Cancelada)
                .SelectMany(r => r.Equipamentos)
                .Where(re => !equipamentoId.HasValue || re.EquipamentoId == equipamentoId.Value)
                .Select(re => new { re.Reserva.HoraInicio, re.Reserva.HoraFim })
                .ToListAsync();

            var horariosDisponiveis = new List<TimeSpan>();
            for (int h = 7; h <= 18; h++)
            {
                var horario = new TimeSpan(h, 0, 0);
                if (!horariosOcupados.Any(o => horario >= o.HoraInicio && horario < o.HoraFim))
                    horariosDisponiveis.Add(horario);
            }
            return horariosDisponiveis;
        }

        public async Task<ReservaDto> CreateAsync(CreateReservaDto dto)
        {
            if (dto.HoraInicio >= dto.HoraFim)
                throw new InvalidOperationException("Hora de início deve ser anterior à hora de fim.");

            if (dto.EquipamentoIds.Count == 0)
                throw new InvalidOperationException("Selecione ao menos um equipamento.");

            var conflitos = await _db.Reservas
                .Include(r => r.Equipamentos)
                .Where(r => r.DataReserva.Date == dto.DataReserva.Date
                    && r.Status != ReservaStatus.Cancelada
                    && r.HoraInicio < dto.HoraFim
                    && r.HoraFim > dto.HoraInicio)
                .SelectMany(r => r.Equipamentos)
                .Where(re => dto.EquipamentoIds.Contains(re.EquipamentoId))
                .Select(re => re.Equipamento.Nome)
                .Distinct()
                .ToListAsync();

            if (conflitos.Count > 0)
                throw new InvalidOperationException(
                    $"Conflito de horário com equipamento(s): {string.Join(", ", conflitos)}");

            var reserva = new Reserva
            {
                NomeSolicitante = dto.NomeSolicitante,
                EmailSolicitante = dto.EmailSolicitante,
                TelefoneSolicitante = dto.TelefoneSolicitante,
                InstituicaoEmpresa = dto.InstituicaoEmpresa,
                DataReserva = dto.DataReserva,
                HoraInicio = dto.HoraInicio,
                HoraFim = dto.HoraFim,
                Observacoes = dto.Observacoes,
                Status = ReservaStatus.Confirmada
            };

            foreach (var eqId in dto.EquipamentoIds)
            {
                reserva.Equipamentos.Add(new ReservaEquipamento
                {
                    ReservaId = reserva.Id,
                    EquipamentoId = eqId
                });
            }

            foreach (var ins in dto.Insumos)
            {
                reserva.Insumos.Add(new ReservaInsumo
                {
                    ReservaId = reserva.Id,
                    InsumoId = ins.InsumoId,
                    QuantidadePrevista = ins.QuantidadePrevista
                });
            }

            _db.Reservas.Add(reserva);
            await _db.SaveChangesAsync();

            return (await GetByIdAsync(reserva.Id))!;
        }

        public async Task<ReservaDto?> UpdateStatusAsync(Guid id, ReservaStatus status)
        {
            var r = await _db.Reservas
                .Include(x => x.Equipamentos).ThenInclude(re => re.Equipamento)
                .Include(x => x.Insumos).ThenInclude(ri => ri.Insumo)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (r is null) return null;

            r.Status = status;
            r.UpdatedAt = DateTime.UtcNow;

            if (status == ReservaStatus.Cancelada || status == ReservaStatus.Concluida)
            {
                foreach (var ri in r.Insumos)
                {
                    ri.QuantidadeUtilizada = ri.QuantidadePrevista;
                }
            }

            await _db.SaveChangesAsync();
            return ToDto(r);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var r = await _db.Reservas.FindAsync(id);
            if (r is null) return false;

            r.IsDeleted = true;
            r.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var hoje = DateTime.UtcNow.Date;
            var inicioSemana = hoje.AddDays(-(int)hoje.DayOfWeek);

            return new DashboardDto(
                TotalReservasHoje: await _db.Reservas
                    .CountAsync(r => r.DataReserva.Date == hoje && !r.IsDeleted),
                TotalReservasSemana: await _db.Reservas
                    .CountAsync(r => r.DataReserva >= inicioSemana && !r.IsDeleted),
                EquipamentosDisponiveis: await _db.Equipamentos
                    .CountAsync(e => e.Status == EquipamentoStatus.Disponivel),
                EquipamentosTotal: await _db.Equipamentos.CountAsync(),
                InsumosEstoqueBaixo: await _db.Insumos
                    .CountAsync(i => i.QuantidadeAtual <= i.QuantidadeMinima),
                InsumosTotal: await _db.Insumos.CountAsync(),
                ReservasPendentes: await _db.Reservas
                    .CountAsync(r => r.Status == ReservaStatus.Pendente && !r.IsDeleted)
            );
        }

        private static ReservaDto ToDto(Reserva r) => new(
            r.Id, r.NomeSolicitante, r.EmailSolicitante,
            r.TelefoneSolicitante, r.InstituicaoEmpresa,
            r.DataReserva, r.HoraInicio, r.HoraFim, r.Status,
            r.Observacoes,
            [.. r.Equipamentos.Select(e => new EquipamentoReservadoDto(e.Id, e.EquipamentoId, e.Equipamento?.Nome ?? ""))],
            [.. r.Insumos.Select(i => new InsumoReservadoDto(i.Id, i.InsumoId, i.Insumo?.Nome ?? "", i.QuantidadePrevista, i.QuantidadeUtilizada))],
            r.CreatedAt
        );
    }
}
