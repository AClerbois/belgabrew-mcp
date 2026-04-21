using System.ComponentModel;
using System.Net.Http.Json;
using ModelContextProtocol.Server;

namespace BelgaBrew.Mcp.Tools;

/// <summary>
/// DEMO 1 — Tool appelant une API publique (Open-Meteo pour la météo de fermentation)
/// Attributs clés :
///   [McpServerToolType]      → déclare la classe comme source de Tools
///   [McpServerTool]          → déclare la méthode comme un Tool MCP
///   [Description]            → texte lu par le LLM pour comprendre quand/comment appeler
/// </summary>
[McpServerToolType]
public class BreweryTools
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IInventoryService _inventory;

    // Injection de dépendances standard .NET — le SDK MCP s'intègre nativement
    public BreweryTools(IHttpClientFactory httpFactory, IInventoryService inventory)
    {
        _httpFactory = httpFactory;
        _inventory = inventory;
    }

    /// <summary>Tool 1 : consultation du stock d'ingrédients</summary>
    [McpServerTool(Name = "check_ingredient_stock")]
    [Description("Retourne le stock actuel d'un ingrédient (houblon, malt, levure) par son SKU.")]
    public async Task<StockInfo> CheckStockAsync(
        [Description("SKU de l'ingrédient, ex: HOP-SAAZ-001")] string sku,
        CancellationToken ct)
    {
        var item = await _inventory.GetAsync(sku, ct);
        return new StockInfo(
            Sku: sku,
            Name: item.Name,
            QuantityKg: item.QuantityKg,
            LowStock: item.QuantityKg < item.ReorderThresholdKg,
            LastUpdated: item.LastUpdated);
    }

    /// <summary>Tool 2 : prévisions météo pour ajuster la fermentation</summary>
    [McpServerTool(Name = "get_fermentation_forecast")]
    [Description("Récupère les prévisions météo 7 jours pour ajuster la fermentation (T° cave non climatisée).")]
    public async Task<FermentationForecast> GetForecastAsync(
        [Description("Latitude, ex: 50.4")] double lat,
        [Description("Longitude, ex: 4.44")] double lon,
        CancellationToken ct)
    {
        var http = _httpFactory.CreateClient();
        // API Open-Meteo — gratuite, sans clé, idéale pour une démo
        var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}" +
                  "&daily=temperature_2m_max,temperature_2m_min&timezone=auto&forecast_days=7";

        var response = await http.GetFromJsonAsync<OpenMeteoResponse>(url, ct);

        var avgMax = response!.Daily.TemperatureMax.Average();
        var avgMin = response.Daily.TemperatureMin.Average();

        // Pattern switch expression — C# 8+
        var recommendation = (avgMax, avgMin) switch
        {
            ( > 25, _) => "Trop chaud pour une ale belge classique. Reporte ou refroidis la cave.",
            ( < 12, _) => "Trop froid, fermentation lente garantie. Chauffe ou choisis une lager.",
            _ => "Conditions OK pour une fermentation ale (16-22°C idéal)."
        };

        return new FermentationForecast(avgMin, avgMax, recommendation);
    }
}

// Records — sérialisés automatiquement en JSON structured content par le SDK
public record StockInfo(
    string Sku,
    string Name,
    double QuantityKg,
    bool LowStock,
    DateTime LastUpdated);

public record FermentationForecast(
    double AvgMinC,
    double AvgMaxC,
    string Recommendation);

// Mapping de la réponse Open-Meteo
internal record OpenMeteoResponse(DailyData Daily);
internal record DailyData(
    [property: System.Text.Json.Serialization.JsonPropertyName("temperature_2m_max")] double[] TemperatureMax,
    [property: System.Text.Json.Serialization.JsonPropertyName("temperature_2m_min")] double[] TemperatureMin);
