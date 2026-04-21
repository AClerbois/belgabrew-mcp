namespace BelgaBrew.Mcp.Services;

public class MockInventoryService : IInventoryService
{
    private static readonly List<InventoryItem> _items = new()
    {
        new("HOP-SAAZ-001",  "Houblon Saaz",          12.5, 20.0, DateTime.UtcNow.AddHours(-3)),
        new("MALT-PILS-001", "Malt Pilsen",           145.0, 50.0, DateTime.UtcNow.AddHours(-6)),
        new("YEAST-T58-001", "Levure Safbrew T-58",    1.2,  2.0, DateTime.UtcNow.AddHours(-1)),
        new("HOP-KENT-001",  "Houblon Kent Goldings",  8.0, 15.0, DateTime.UtcNow.AddDays(-1)),
        new("MALT-CARA-001", "Malt Caramel",          22.0, 10.0, DateTime.UtcNow.AddHours(-12)),
    };

    public Task<InventoryItem> GetAsync(string sku, CancellationToken ct = default)
    {
        var item = _items.FirstOrDefault(i => i.Sku.Equals(sku, StringComparison.OrdinalIgnoreCase))
            ?? new InventoryItem(sku, "Ingrédient inconnu", 0, 0, DateTime.UtcNow);
        return Task.FromResult(item);
    }

    public Task<List<InventoryItem>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult(_items);

    public Task<List<InventoryItem>> GetLowStockAsync(CancellationToken ct = default)
        => Task.FromResult(_items.Where(i => i.QuantityKg < i.ReorderThresholdKg).ToList());
}
