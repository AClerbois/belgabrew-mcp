namespace BelgaBrew.Mcp.Services;

public interface IBrewingService
{
    Task<string> LaunchAsync(string recipeId, int volumeLiters, string responsible, string priority, CancellationToken ct = default);
}
