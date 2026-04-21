namespace BelgaBrew.Mcp.Services;

public class MockBrewingService : IBrewingService
{
    public Task<string> LaunchAsync(string recipeId, int volumeLiters, string responsible, string priority, CancellationToken ct = default)
    {
        var brewId = $"BREW-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
        Console.WriteLine($"[BelgaBrew] Brassin lancé: {brewId} | {recipeId} | {volumeLiters}L | {responsible} | {priority}");
        return Task.FromResult(brewId);
    }
}
