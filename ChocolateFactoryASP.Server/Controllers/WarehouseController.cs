using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChocolateFactory.Services;
using ChocolateFactory.Models;
using System.ComponentModel.DataAnnotations;
using ChocolateFactory.Requests;

namespace ChocolateFactory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WarehouseController : ControllerBase
    {
        private readonly WarehouseService _service;
        private readonly PackagingService _finishedGoodService;

        public WarehouseController(WarehouseService service, PackagingService finishedGoodService)
        {
            _service = service;
            _finishedGoodService = finishedGoodService;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllWarehouses()
        {
            var warehouses = await _service.GetAllWarehousesAsync();
            return Ok(warehouses);
        }

        [HttpPost]
        [Authorize(Roles ="FactoryManager")]
        public async Task<IActionResult> AddWarehouseAsync([FromBody] WarehouseRequest warehouse)
        {

            var warehouses = await _service.GetAllWarehousesAsync();

            var alreadyExists = warehouses.Find(x=>x.Name == warehouse.Name);

            if (alreadyExists != null) {
                return BadRequest("Warehouse name already exists, please use unique names");
            }

            Warehouse temp = new Warehouse
            {
                Location = warehouse.Location,
                Name = warehouse.Name,
                Capacity = warehouse.Capacity,
                CurrentStockLevel = warehouse.CurrentStockLevel
            };

            await _service.AddWarehouseAsync(temp);
            return Ok(warehouse);
        }

        [HttpPut]
        [Authorize(Roles = "FactoryManager")]
        public async Task<IActionResult> UpdateWarehouseAsync(Warehouse warehouse)
        {
            var exists = await _service.GetWarehouseByNameAsync(warehouse.Name);

            if (exists == null)
            {
                return BadRequest("Warehouse with Name: " + warehouse.Name + " doesn't exist");
            }

            await _service.UpdateWarehouseAsync(warehouse);
            return Ok(new { message = "Warehouse Updated Successfully" });
        }

        [HttpDelete("{name}")]
        [Authorize(Roles = "FactoryManager")]
        public async Task<IActionResult> DeleteWarehouseAsync(string name)
        {
            var exists = await _service.GetWarehouseByNameAsync(name);

            if (exists == null)
            {
                return BadRequest("Warehouse with Name: " + name + " doesn't exist");
            }

            await _service.DeleteWarehouseAsync(name);
            return Ok(new { message = "Warehouse Deleted Successfully" });
        }

        [HttpGet("sufficient-stock/{quantity}")]
        public async Task<IActionResult> GetWarehousesWithSufficientStock(int quantity)
        {
            if (quantity <= 0)
                return BadRequest("Quantity must be greater than 0.");

            var warehouses = await _service.GetAllWarehousesAsync();

            // Filter warehouses with enough stock
            var filteredWarehouses = warehouses
                .Where(w => w.Capacity - w.CurrentStockLevel >= quantity)
                .ToList();

            return Ok(filteredWarehouses);
        }

        [HttpPut("deduct-stock/{productId}/{quantity}")]
        public async Task<IActionResult> DeductStock(Guid productId, int quantity)
        {
            FinishedGood good = await _finishedGoodService.GetFinishedGoodByIdAsync(productId);
            string warehouse = good.WarehouseLocation;

            var warehouseItem = await _service.GetWarehouseByNameAsync(warehouse);

            if (warehouseItem == null)
            {
                return NotFound(new { message = "Product not found in warehouse." });
            }

            if (warehouseItem.CurrentStockLevel < quantity)
            {
                return BadRequest(new { message = "Not enough stock available." });
            }

            warehouseItem.CurrentStockLevel -= quantity;
            await _service.UpdateWarehouseAsync(warehouseItem);

            return Ok(new { message = "Stock updated successfully." });
        }
    }
}