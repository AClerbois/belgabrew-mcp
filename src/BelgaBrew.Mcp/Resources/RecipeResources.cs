using System.ComponentModel;
using System.Text.Json;
using BelgaBrew.Mcp.Services;
using ModelContextProtocol.Server;

namespace BelgaBrew.Mcp.Resources;

[McpServerResourceType]
public class RecipeResources(IRecipeRepository repo)
{
    [McpServerResource(
        UriTemplate = "brewery://recipes",
        Name = "Catalogue des recettes",
        MimeType = "application/json")]
    [Description("Liste complète des recettes actives de la brasserie.")]
    public async Task<string> GetAllRecipes()
    {
        var recipes = await repo.GetAllAsync();
        return JsonSerializer.Serialize(recipes, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerResource(
        UriTemplate = "brewery://recipes/{recipeId}",
        Name = "Fiche recette détaillée",
        MimeType = "application/json")]
    [Description("Fiche technique complète d'une recette : ingrédients, étapes, température, durée.")]
    public async Task<string> GetRecipeById(
        [Description("Identifiant unique de la recette, ex: triple-abbaye-2026")] string recipeId)
    {
        var recipe = await repo.GetByIdAsync(recipeId);
        return JsonSerializer.Serialize(recipe, new JsonSerializerOptions { WriteIndented = true });
    }
}
