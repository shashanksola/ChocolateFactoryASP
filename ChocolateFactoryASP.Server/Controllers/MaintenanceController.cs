using ChocolateFactory.Models;
using ChocolateFactory.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChocolateFactory.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MaintenanceController : ControllerBase
    {
        private readonly MaintenanceService _service;

        public MaintenanceController(MaintenanceService service)
        {
            _service = service;
        }

        // Allow Technicians and FactoryManagers to view records
        [HttpGet]
        public async Task<IActionResult> GetAllMaintenanceRecords()
        {
            var records = await _service.GetAllMaintenanceRecordsAsync();
            return Ok(records);
        }

        // Allow only Technicians to add new records
        [HttpPost]
        [Authorize(Roles = "FactoryManager,Technician")]
        public async Task<IActionResult> AddMaintenanceRecordAsync([FromBody] MaintenanceRecord record)
        {
            await _service.AddMaintenanceRecordAsync(record);
            return Ok(record);
        }

        // Allow only FactoryManagers to update records
        [HttpPut]
        [Authorize(Roles = "FactoryManager,Technician")]
        public async Task<IActionResult> UpdateMaintenanceRecordAsync([FromBody] MaintenanceRecord record)
        {
            var rec = await _service.GetMaintenanceRecordByIdAsync(record.RecordId);

            if (rec == null)
            {
                return BadRequest("No Record Exists");
            }

            await _service.UpdateMaintenenceRecordAsync(record);
            return NoContent();
        }

        // Allow only FactoryManagers to delete records
        [HttpDelete("{id}")]
        [Authorize(Roles ="FactoryManager")]
        public async Task<IActionResult> DeleteMaintenanceRecordAsync(Guid id)
        {
            var rec = await _service.GetMaintenanceRecordByIdAsync(id);

            if (rec == null)
            {
                return BadRequest("No Record Exists");
            }

            await _service.DeleteMaintaneneceTask(id);
            return NoContent();
        }
    }

}