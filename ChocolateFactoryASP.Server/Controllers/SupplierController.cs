using ChocolateFactory.Models;
using ChocolateFactory.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChocolateFactory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "FactoryManager")]
    public class SupplierController : ControllerBase
    {
        private readonly SupplierService _service;

        public SupplierController(SupplierService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "FactoryManager,MaterialStaff,ProductionSupervisor")]
        public async Task<IActionResult> GetAllSuppliers()
        {
            var suppliers = await _service.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "FactoryManager,MaterialStaff,ProductionSupervisor")]
        public async Task<IActionResult> GetSupplierById(Guid id)
        {
            var supplier = await _service.GetSupplierByIdAsync(id);
            if (supplier == null)
                return NotFound($"Supplier with ID {id} not found.");
            return Ok(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier([FromBody] Supplier supplier)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.AddSupplierAsync(supplier);
            return CreatedAtAction(nameof(GetSupplierById), new { id = supplier.SupplierId }, supplier);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(Guid id, [FromBody] Supplier supplier)
        {
            if (id != supplier.SupplierId)
                return BadRequest("Supplier ID mismatch.");

            try
            {
                await _service.UpdateSupplierAsync(supplier);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            await _service.DeleteSupplierAsync(id);
            return NoContent();
        }
    }
}
