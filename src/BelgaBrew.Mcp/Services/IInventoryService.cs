namespace BelgaBrew.Mcp.Services;

public interface IInventoryService
{
    Task<InventoryItem> GetAsync(string sku, CancellationToken ct = default);
    Task<List<InventoryItem>> GetAllAsync(CancellationToken ct = default);
    Task<List<InventoryItem>> GetLowStockAsync(CancellationToken ct = default);
}

public record InventoryItem(
    string Sku,
    string Name,
    double QuantityKg,
    double ReorderThresholdKg,
    DateTime LastUpdated);
