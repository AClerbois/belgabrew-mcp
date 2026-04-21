using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace BelgaBrew.Mcp.Resources;

/// <summary>
/// DEMO 2 — Resources MCP
/// Différence clé avec les Tools :
///   - Une Resource est LUE (contexte, données statiques ou semi-statiques)
///   - Un Tool a des EFFETS (fait quelque chose)
///   - URI scheme custom : brewery://
/// </summary>
[McpServerResourceType]
public class RecipeResources
{
    private readonly IRecipeRepository _repo;

    public RecipeResources(IRecipeRepository repo) => _repo = repo;

    /// <summary>
    /// Resource statique : le catalogue complet des recettes
    /// URI fixe — brewery://recipes
    /// </summary>
    [McpServerResource(
        Uri = "brewery://recipes",
        Name = "Catalogue des recettes",
        MimeType = "application/json")]
    [Description("Liste complète des recettes actives de la brasserie.")]
    public async Task<string> GetAllRecipes()
    {
        var recipes = await _repo.GetAllAsync();
        return JsonSerializer.Serialize(recipes, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Resource dynamique : fiche par recipeId
    /// URI template — le {recipeId} est résolu automatiquement
    /// </summary>
    [McpServerResource(
        UriTemplate = "brewery://recipes/{recipeId}",
        Name = "Fiche recette détaillée",
        MimeType = "application/json")]
    [Description("Fiche technique complète d'une recette : ingrédients, étapes, température, durée.")]
    public async Task<string> GetRecipeById(
        [Description("Identifiant unique de la recette, ex: triple-abbaye-2026")] string recipeId)
    {
        var recipe = await _repo.GetByIdAsync(recipeId);
        return JsonSerializer.Serialize(recipe, new JsonSerializerOptions { WriteIndented = true });
    }
}
