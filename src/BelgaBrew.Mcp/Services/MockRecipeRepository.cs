namespace BelgaBrew.Mcp.Services;

public class MockRecipeRepository : IRecipeRepository
{
    private static readonly List<RecipeDetail> _recipes = new()
    {
        new RecipeDetail(
            "triple-abbaye-2026", "Triple Abbaye 2026", "Belgian Tripel",
            "Tripel dorée, refermentée en bouteille, 9.5% ABV.",
            new List<RecipeIngredient>
            {
                new("MALT-PILS-001", "Malt Pilsen", 18.0),
                new("HOP-SAAZ-001",  "Houblon Saaz", 0.4),
                new("YEAST-T58-001", "Levure T-58",  0.015)
            },
            FermentationTempC: 20, DurationDays: 21),
        new RecipeDetail(
            "blonde-charleroi", "Blonde de Charleroi", "Belgian Blonde",
            "Blonde légère et fruitée, 6.2% ABV, notes d'agrumes.",
            new List<RecipeIngredient>
            {
                new("MALT-PILS-001", "Malt Pilsen",   14.0),
                new("MALT-CARA-001", "Malt Caramel",   1.5),
                new("HOP-KENT-001",  "Kent Goldings",  0.3),
            },
            FermentationTempC: 18, DurationDays: 14),
    };

    public Task<List<RecipeSummary>> GetAllAsync()
        => Task.FromResult(_recipes.Select(r => new RecipeSummary(r.Id, r.Name, r.Style, true)).ToList());

    public Task<RecipeDetail> GetByIdAsync(string recipeId)
    {
        var r = _recipes.FirstOrDefault(r => r.Id.Equals(recipeId, StringComparison.OrdinalIgnoreCase))
            ?? throw new KeyNotFoundException($"Recette '{recipeId}' introuvable.");
        return Task.FromResult(r);
    }
}
