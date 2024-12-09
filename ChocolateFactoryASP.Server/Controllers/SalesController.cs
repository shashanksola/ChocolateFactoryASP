using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChocolateFactory.Services;
using ChocolateFactory.Models;
using ChocolateFactory.Data;
using Microsoft.EntityFrameworkCore;

namespace ChocolateFactory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SalesRepresentative,FactoryManager")]
    public class SalesController : ControllerBase
    {
        private readonly SalesService _service;
        private readonly ApplicationDbContext _context;

        public SalesController(SalesService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
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

            Warehouse ware = await _service.GetWarehouseFromFinishedGoodId(order.ProductId);
            if (ware != null && ware.CurrentStockLevel - order.Quantity >= 0) {
                return BadRequest(new { message = "Warehouse doesnt have the capacity" });
            }

            FinishedGood good = await _service.GetFinishedGoodById(order.ProductId);
            if (good != null && good.Quantity < order.Quantity) {
                return BadRequest(new { message = "Finished Good doesnt have the Quantity" });
            }

            await _service.AddOrderAsync(order);

            await _service.DeductWarehouseStockAsync(order.ProductId, order.Quantity);

            await _service.DeductFinishedGoodStockAsync(order.ProductId, order.Quantity);

            return Ok(order);
        }

    }
}