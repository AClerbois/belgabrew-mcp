namespace BelgaBrew.Mcp.Services;

public class MockBrewingService : IBrewingService
{
    private static readonly List<ActiveBrew> _activeBrews =
    [
        new("BREW-20250601-1234", "triple-abbaye-2026",  300, "Jean",  "Fermentation", DateTime.UtcNow.AddDays(-5)),
        new("BREW-20250603-5678", "blonde-charleroi",    500, "Marie", "Garde",         DateTime.UtcNow.AddDays(-2)),
    ];

    public Task<string> LaunchAsync(string recipeId, int volumeLiters, string responsible, string priority, CancellationToken ct = default)
    {
        var brewId = $"BREW-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
        Console.WriteLine($"[BelgaBrew] Brassin lancé: {brewId} | {recipeId} | {volumeLiters}L | {responsible} | {priority}");
        return Task.FromResult(brewId);
    }

    public Task<List<ActiveBrew>> GetActiveBrewsAsync(CancellationToken ct = default)
        => Task.FromResult(_activeBrews);
}
