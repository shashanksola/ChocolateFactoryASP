using ChocolateFactory.Data;
using ChocolateFactory.Models;
using Microsoft.EntityFrameworkCore;

namespace ChocolateFactory.Repositories
{

    public interface IRecipeRepository
    {
        Task<Recipe> GetRecipeByIdAsync(string name);
        Task<IEnumerable<Recipe>> GetAllRecipesAsync();
        Task AddRecipeAsync(Recipe recipe);
        Task UpdateRecipeAsync(string name, Recipe recipe);
        Task DeleteRecipeAsync(string name);
    }


    public class RecipeRepository : IRecipeRepository
    {
        private readonly ApplicationDbContext _context;

        public RecipeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Recipe> GetRecipeByIdAsync(string name) =>
            await _context.Recipes.FindAsync(name);

        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync() =>
            await _context.Recipes.ToListAsync();

        public async Task AddRecipeAsync(Recipe recipe)
        {
            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRecipeAsync(string name, Recipe recipe)
        {
            var recipes = await GetRecipeByIdAsync(name);

            if (recipes == null) {
                _context.Recipes.Update(recipe);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRecipeAsync(string name)
        {
            var recipe = await GetRecipeByIdAsync(name);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
        }
    }

}
