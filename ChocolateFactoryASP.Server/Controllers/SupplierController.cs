using ChocolateFactory.Models;
using ChocolateFactory.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChocolateFactory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "FactoryManager,Admin")]
    public class SupplierController : ControllerBase
    {
        private readonly SupplierService _service;

        public SupplierController(SupplierService service)
        {
            _service = service;
        }

        // GET: api/Supplier
        [HttpGet]
        public async Task<IActionResult> GetAllSuppliers()
        {
            var suppliers = await _service.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        // GET: api/Supplier/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierById(Guid id)
        {
            var supplier = await _service.GetSupplierByIdAsync(id);
            if (supplier == null)
                return NotFound($"Supplier with ID {id} not found.");
            return Ok(supplier);
        }

        // POST: api/Supplier
        [HttpPost]
        public async Task<IActionResult> AddSupplier([FromBody] Supplier supplier)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.AddSupplierAsync(supplier);
            return CreatedAtAction(nameof(GetSupplierById), new { id = supplier.SupplierId }, supplier);
        }

        // PUT: api/Supplier/{id}
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

        // DELETE: api/Supplier/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            await _service.DeleteSupplierAsync(id);
            return NoContent();
        }
    }
}
