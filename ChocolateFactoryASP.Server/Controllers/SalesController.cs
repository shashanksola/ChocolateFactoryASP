using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChocolateFactory.Services;
using ChocolateFactory.Models;

namespace ChocolateFactory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SalesRepresentative,FactoryManager")]
    public class SalesController : ControllerBase
    {
        private readonly SalesService _service;

        public SalesController(SalesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _service.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] SalesOrder order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Begin transaction
            using (var transaction = await _service.BeginTransactionAsync())
            {
                try
                {
                    // Add the order
                    await _service.AddOrderAsync(order);

                    // Deduct stock in Warehouse
                    await _service.DeductWarehouseStockAsync(order.ProductId, order.Quantity);

                    // Deduct stock in FinishedGood table
                    await _service.DeductFinishedGoodStockAsync(order.ProductId, order.Quantity);

                    // Commit transaction
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // Rollback transaction on error
                    await transaction.RollbackAsync();
                    return BadRequest($"Failed to place order: {ex.Message}");
                }
            }

            return Ok(order);
        }

    }
}