using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChocolateFactory.Services;
using ChocolateFactory.Models;

namespace   ChocolateFactory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "FactoryManager,SalesRepresentative")]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _service;

        public ReportController(ReportService service)
        {
            _service = service;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport()
        {
            var report = await _service.GenerateSalesReportAsync();
            return Ok(report);
        }

        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventoryReport()
        {
            var report = await _service.GenerateInventoryReportAsync();
            return Ok(report);
        }
    }
}