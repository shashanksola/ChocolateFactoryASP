using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChocolateFactory.Services;
using ChocolateFactory.Models;

namespace ChocolateFactory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "PackagingStaff,FactoryManager")]
    public class PackagingController : ControllerBase
    {
        private readonly PackagingService _service;
        private readonly ProductionService _production;
        private readonly RecipeService _recipe;
        private readonly WarehouseService _warehouse;

        public PackagingController(PackagingService service, ProductionService production, RecipeService recipe, WarehouseService warehouseService)
        {
            _service = service;
            _production = production;
            _recipe = recipe;
            _warehouse = warehouseService;
        }

        [HttpGet]
        [Authorize(Roles = "SalesRepresentative")]
        public async Task<IActionResult> GetAllFinishedGoods()
        {
            var goods = await _service.GetAllFinishedGoodsAsync();
            return Ok(goods);
        }


        [HttpPost]
        public async Task<IActionResult> AddFinishedGoodAsync([FromBody] FinishedGood finishedGood)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Add the finished good
            await _service.AddFinishedGoodAsync(finishedGood);

            // Update warehouse current stock level
            var warehouse = await _warehouse.GetWarehouseByNameAsync(finishedGood.WarehouseLocation);

            if (warehouse == null)
            {
                return NotFound($"Warehouse with location '{finishedGood.WarehouseLocation}' not found.");
            }

            warehouse.CurrentStockLevel += finishedGood.Quantity; // Update stock quantity
            await _warehouse.UpdateWarehouseAsync(warehouse);

            return Ok(finishedGood);
        }


        [HttpGet("qunatity/{id}")]
        public async Task<int> GetRecipeQuantityFromBatchId(Guid id)
        {
            ProductionSchedule schedule = await _production.GetScheduleByIDAsync(id);
            string RecipeName = schedule.RecipeName;

            Recipe recipe = await _recipe.GetRecipeByNameAsync(RecipeName);

            return recipe.QuantityPerBatch;
        }
    }
}