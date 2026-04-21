namespace BelgaBrew.Mcp.Services;

public interface IRecipeRepository
{
    Task<List<RecipeSummary>> GetAllAsync();
    Task<RecipeDetail> GetByIdAsync(string recipeId);
}

public record RecipeSummary(string Id, string Name, string Style, bool IsActive);

public record RecipeDetail(
    string Id,
    string Name,
    string Style,
    string Description,
    List<RecipeIngredient> Ingredients,
    double FermentationTempC,
    int DurationDays);

public record RecipeIngredient(string Sku, string Name, double QuantityKgPer100L);
