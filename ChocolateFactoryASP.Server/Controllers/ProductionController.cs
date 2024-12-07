using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChocolateFactory.Services;
using ChocolateFactory.Models;

namespace ChocolateFactory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ProductionSupervisor,FactoryManager,QualityController")]
    public class ProductionController : ControllerBase
    {
        private readonly ProductionService _service;
        private readonly RecipeService _recipeService;
        private readonly RawMaterialService _rawMaterialService;
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;

        public ProductionController(ProductionService service, RecipeService recipeService, RawMaterialService rawMaterialService, UserService userService, NotificationService notificationService)
        {
            _service = service;
            _recipeService = recipeService;
            _rawMaterialService = rawMaterialService;
            _userService = userService;
            _notificationService = notificationService;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllSchedules()
        {
            var schedules = await _service.GetAllSchedulesAsync();
            return Ok(schedules);
        }

        [HttpGet("/InProgress")]
        public async Task<IActionResult> GetAllActiveSchedules()
        {
            var schedules = await _service.GetAllSchedulesAsync();
            schedules = schedules.Where(x => x.Status == ProductionStatus.InProgress);
            return Ok(schedules);
        }

        [HttpGet("/Completed")]
        [Authorize(Roles ="FactoryManager,QualityController")]
        public async Task<IActionResult> GetAllCompletedSchedules()
        {
            var schedules = await _service.GetAllSchedulesAsync();
            schedules = schedules.Where(x => x.Status == ProductionStatus.Completed);
            return Ok(schedules);
        }


        [HttpPost]
        public async Task<IActionResult> AddSchedule([FromBody] ProductionSchedule schedule)
        {
            // Fetch the recipe based on the provided recipe name
            Recipe recipe = await _recipeService.GetRecipeByNameAsync(schedule.RecipeName);

            if (recipe == null)
            {
                return BadRequest(new { message = "Cannot find recipe with name: " + schedule.RecipeName });
            }

            // Parse the ingredients from the recipe
            string ingredient = recipe.Ingredients;
            List<Ingredient> ingredients = _recipeService.ParseIngredientsFromString(ingredient);

            // Fetch all raw materials from the warehouse
            List<RawMaterial> allMaterials = await _rawMaterialService.GetAllRawMaterialsAsync();

            // Track updates to raw materials
            foreach (var requiredIngredient in ingredients)
            {
                // Get all batches of the raw material required for the ingredient
                var materialBatches = allMaterials
                    .Where(mat => string.Equals(mat.Name, requiredIngredient.IngredientName, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(mat => mat.ExpiryDate) // Use FIFO based on expiration date
                    .ToList();

                if (!materialBatches.Any())
                {
                    return BadRequest(new
                    {
                        message = $"Raw material {requiredIngredient.IngredientName} not available in stock."
                    });
                }

                double requiredQuantity = requiredIngredient.Quantity;
                foreach (var batch in materialBatches)
                {
                    if (requiredQuantity <= 0)
                        break;

                    if (batch.StockQuantity >= Math.Ceiling(requiredQuantity))
                    {
                        // Deduct the required quantity from this batch
                        batch.StockQuantity -= (int)Math.Ceiling(requiredQuantity);
                        requiredQuantity = 0;
                    }
                    else
                    {
                        // Use all stock from this batch and move to the next
                        requiredQuantity -= batch.StockQuantity;
                        batch.StockQuantity = 0;
                    }
                }

                // If after processing all batches the required quantity is not met
                if (requiredQuantity > 0)
                {
                    return BadRequest(new
                    {
                        message = $"Insufficient stock for {requiredIngredient.IngredientName}. Missing {requiredQuantity} units."
                    });
                }
            }

            // Update the modified batches in the database
            foreach (var batch in allMaterials)
            {
                await _rawMaterialService.UpdateRawMaterialAsync(batch);
            }

            // Add the production schedule
            await _service.AddScheduleAsync(schedule);

            return Ok(schedule);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> CompleteProductionByScheduleID(Guid id)
        {
            var schedule = await _service.GetScheduleByIDAsync(id);

            schedule.Status = ProductionStatus.Completed;

            await _service.UpdateScheduleAsync(schedule);

            IEnumerable<User> users = await _userService.GetUsersByUserRoleAsync(UserRole.QualityController);

            // Prepare email content
            string subject = "New Production Completed!";
            string body = $"A new Production has been completed in the Chocolate Factory system.";

            // Send email notifications
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.Email))
                {
                    await _notificationService.SendEmailAsync(user.Email, subject, body);
                }
            }

            return Ok(schedule);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductionByScheduleID(Guid id, ProductionSchedule InputSchedule)
        {
            var schedule = await _service.GetScheduleByIDAsync(id);

            schedule.Shift = InputSchedule.Shift;
            schedule.SupervisorId = InputSchedule.SupervisorId;
            schedule.StartDate = InputSchedule.StartDate;
            schedule.EndDate = InputSchedule.EndDate;
            schedule.RecipeName = InputSchedule.RecipeName;
            schedule.Status = InputSchedule.Status;

            await _service.UpdateScheduleAsync(schedule);

            return Ok(schedule);
        }
    }
}