using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChocolateFactory.Services;
using ChocolateFactory.Models;

namespace ChocolateFactory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RawMaterialController : ControllerBase
    {
        private readonly RawMaterialService _service;

        public RawMaterialController(RawMaterialService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMaterials()
        {
            var materials = await _service.GetAllRawMaterialsAsync();
            return Ok(materials);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetMaterialByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Material name cannot be empty.");

            var material = await _service.GetRawMaterialByNameAsync(name);
            if (material == null)
                return NotFound($"Raw material with name '{name}' not found.");

            return Ok(material);
        }

        [HttpGet("batch/{id}")]
        public async Task<IActionResult> GetMaterialByBatchId(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Batch ID cannot be empty.");

            var material = await _service.GetRawMaterialByBatchIdAsync(id);
            if (material == null)
                return NotFound($"Raw material with batch ID '{id}' not found.");

            return Ok(material);
        }

        [HttpGet("warehouse/{warehouseName}")]
        public async Task<IActionResult> GetMaterialsByWarehouse(string warehouseName)
        {
            if (string.IsNullOrWhiteSpace(warehouseName))
                return BadRequest("Warehouse name cannot be empty.");

            var materials = await _service.GetAllRawMaterialsByWarehouseNameAsync(warehouseName);
            return Ok(materials);
        }

        [HttpPost]
        public async Task<IActionResult> AddMaterial([FromBody] RawMaterial material)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.AddRawMaterialAsync(material);
            return CreatedAtAction(nameof(GetMaterialByName), new { name = material.Name }, material);
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> UpdateMateriaByName(string name, [FromBody] RawMaterial material)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (name != material.Name)
                return BadRequest("Material Name mismatch.");

            await _service.UpdateRawMaterialAsync(material);
            return NoContent();
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteMaterialByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Material name cannot be empty.");

            await _service.DeleteRawMaterialAsync(name);
            return NoContent();
        }

        [HttpDelete("batch/{id}")]
        public async Task<IActionResult> DeleteMaterialByBatch(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Batch ID cannot be empty.");

            await _service.DeleteRawMaterialByBatchAsync(id);
            return NoContent();
        }
    }
}
