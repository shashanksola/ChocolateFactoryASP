using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChocolateFactory.Services;
using ChocolateFactory.Models;
using System.ComponentModel.DataAnnotations;

namespace ChocolateFactory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecipeController : ControllerBase
    {
        private readonly RecipeService _service;
        private readonly NotificationService _notificationService;
        private readonly UserService _userService;

        public RecipeController(RecipeService service, NotificationService notificationService, UserService userService)
        {
            _service = service;
            _notificationService = notificationService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRecipes()
        {
            var recipes = await _service.GetAllRecipesAsync();
            return Ok(recipes);
        }

        [HttpPost]
        public async Task<IActionResult> AddRecipe([FromBody] RecipeRequest recipe)
        {
            await _service.AddRecipeAsync(_service.getRecipeFromRecipeRequest(recipe));


            // Fetch all users
            IEnumerable<User> users = await _userService.GetUsersAsync();

            // Prepare email content
            string subject = "New Recipe Added!";
            string body = $"A new recipe '{recipe.Name}' has been added to the Chocolate Factory system.";

            // Send email notifications
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.Email))
                {
                    await _notificationService.SendEmailAsync(user.Email, subject, body);
                }
            }

            return Ok(new { Message = "Recipe added and notifications sent successfully!" });
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> UpdateRecipeAsync(string name, [FromBody] RecipeRequest updatedRecipe)
        {   
            
            await _service.UpdateRecipeAsync(name, _service.getRecipeFromRecipeRequest(updatedRecipe));
            return NoContent();
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteRecipeAsync(string name)
        {
            await _service.DeleteRecipeAsync(name);
            return NoContent();
        }
    }

    public class RecipeRequest
    {
        [Key]
        [Required]
        public required string Name { get; set; }

        public required List<Ingredient> Ingredients { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity per batch must be at least 1.")]
        public required int QuantityPerBatch { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Instructions cannot exceed 1000 characters.")]
        public required string Instructions { get; set; }
    }

    public class Ingredient {
        public required string IngredientName { get; set; }
        public required double Quantity { get; set; }
        public required Unit Unit { get; set; }
    }
}