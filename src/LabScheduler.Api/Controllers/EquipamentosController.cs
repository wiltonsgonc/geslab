using LabScheduler.Domain.DTOs;
using LabScheduler.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabScheduler.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipamentosController(EquipamentoService service) : ControllerBase
    {
        private readonly EquipamentoService _service = service;

        [HttpGet]
        public async Task<ActionResult<List<EquipamentoDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EquipamentoDto>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<EquipamentoDto>> Create(CreateEquipamentoDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EquipamentoDto>> Update(Guid id, UpdateEquipamentoDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            return await _service.DeleteAsync(id) ? NoContent() : NotFound();
        }
    }
}
