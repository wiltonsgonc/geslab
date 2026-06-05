using LabScheduler.Domain.DTOs;
using LabScheduler.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabScheduler.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsumosController(InsumoService service) : ControllerBase
    {
        private readonly InsumoService _service = service;

        [HttpGet]
        public async Task<ActionResult<List<InsumoDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InsumoDto>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<InsumoDto>> Create(CreateInsumoDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InsumoDto>> Update(Guid id, UpdateInsumoDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            return await _service.DeleteAsync(id) ? NoContent() : NotFound();
        }

        [HttpGet("estoque-baixo")]
        public async Task<ActionResult<List<InsumoDto>>> GetEstoqueBaixo()
        {
            return Ok(await _service.GetEstoqueBaixoAsync());
        }
    }
}
