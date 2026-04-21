namespace BelgaBrew.Mcp.Services;

public interface IBrewingService
{
    Task<string> LaunchAsync(string recipeId, int volumeLiters, string responsible, string priority, CancellationToken ct = default);
    Task<List<ActiveBrew>> GetActiveBrewsAsync(CancellationToken ct = default);
}

public record ActiveBrew(string BrewId, string RecipeId, int VolumeLiters, string Responsible, string Status, DateTime StartedAt);
