using ChocolateFactory.Controllers;
using ChocolateFactory.Models;
using ChocolateFactory.Repositories;

namespace ChocolateFactory.Services
{
    public class RecipeService
    {
        private readonly RecipeRepository _repository;

        public RecipeService(RecipeRepository repository)
        {
            _repository = repository;
        }

        public string parseIngredients(List<Ingredient> li)
        {
            string ing = "";

            for (int i = 0; i < li.Count; i++)
            {
                Ingredient ingr = li[i];
                ing += ingr.IngredientName + " " + ingr.Quantity.ToString() + " " + ingr.Unit.ToString() + ",";
            }

            return ing;
        }

        public List<Ingredient> ParseIngredientsFromString(string input)
        {
            string[] ingredientParts = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
            List<Ingredient> ingredients = new List<Ingredient>();

            foreach (var part in ingredientParts)
            {
                string[] details = part.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (details.Length < 3)
                {
                    throw new ArgumentException("Invalid ingredient format. Each ingredient must have a name, quantity, and unit.");
                }

                string ingredientName = string.Join(" ", details.Take(details.Length - 2));
                double quantity;
                string unitString = details.Last();

                if (!double.TryParse(details[details.Length - 2], out quantity))
                {
                    throw new ArgumentException($"Invalid quantity for ingredient: {part}");
                }

                if (!Enum.TryParse(unitString, true, out Unit unit))
                {
                    throw new ArgumentException($"Invalid unit for ingredient: {unitString}");
                }

                ingredients.Add(new Ingredient
                {
                    IngredientName = ingredientName,
                    Quantity = quantity,
                    Unit = unit
                });
            }

            return ingredients;
        }


        public Recipe getRecipeFromRecipeRequest(RecipeRequest request) {
            Recipe recipe = new()
            {
                Name = request.Name,
                Ingredients = parseIngredients(request.Ingredients),
                QuantityPerBatch = request.QuantityPerBatch,
                Instructions = request.Instructions,
            };

            return recipe;
        }

        public async Task<IEnumerable<RecipeRequest>> GetAllRecipesAsync()
        {
            IEnumerable<Recipe> recipes = await _repository.GetAllRecipesAsync();

            IEnumerable<RecipeRequest> recipeRequests = recipes.Select(recipe => new RecipeRequest
            {
                Name = recipe.Name,
                Ingredients = ParseIngredientsFromString(recipe.Ingredients),
                QuantityPerBatch=recipe.QuantityPerBatch,
                Instructions = recipe.Instructions
            });

            return recipeRequests;
        }

        public async Task AddRecipeAsync(Recipe recipe)
        {
            await _repository.AddRecipeAsync(recipe);
        }

        public async Task UpdateRecipeAsync(string name, Recipe updatedRecipe)
        {
            await _repository.UpdateRecipeAsync(name, updatedRecipe);
        }

        public async Task DeleteRecipeAsync(string name)
        {
            await _repository.DeleteRecipeAsync(name);
        }

        public async Task<Recipe> GetRecipeByNameAsync(string name)
        {
            return await _repository.GetRecipeByIdAsync(name);
        }
    }
}
