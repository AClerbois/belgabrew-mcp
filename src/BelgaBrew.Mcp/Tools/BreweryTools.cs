using System.ComponentModel;
using System.Net.Http.Json;
using BelgaBrew.Mcp.Services;
using ModelContextProtocol.Server;

namespace BelgaBrew.Mcp.Tools;

[McpServerToolType]
public class BreweryTools(IHttpClientFactory httpFactory, IInventoryService inventory)
{
    [McpServerTool(Name = "check_ingredient_stock")]
    [Description("Retourne le stock actuel d'un ingrédient (houblon, malt, levure) par son SKU.")]
    public async Task<StockInfo> CheckStockAsync(
        [Description("SKU de l'ingrédient, ex: HOP-SAAZ-001")] string sku,
        CancellationToken ct)
    {
        var item = await inventory.GetAsync(sku, ct);
        return new StockInfo(sku, item.Name, item.QuantityKg,
            item.QuantityKg < item.ReorderThresholdKg, item.LastUpdated);
    }

    [McpServerTool(Name = "get_fermentation_forecast")]
    [Description("Récupère les prévisions météo 7 jours pour ajuster la fermentation (T° cave non climatisée).")]
    public async Task<FermentationForecast> GetForecastAsync(
        [Description("Latitude, ex: 50.4")] double lat,
        [Description("Longitude, ex: 4.44")] double lon,
        CancellationToken ct)
    {
        var http = httpFactory.CreateClient();
        var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}" +
                  "&daily=temperature_2m_max,temperature_2m_min&timezone=auto&forecast_days=7";

        var response = await http.GetFromJsonAsync<OpenMeteoResponse>(url, ct);

        var avgMax = response!.Daily.TemperatureMax.Average();
        var avgMin = response.Daily.TemperatureMin.Average();

        var recommendation = (avgMax, avgMin) switch
        {
            ( > 25, _) => "Trop chaud pour une ale belge classique. Reporte ou refroidis la cave.",
            ( < 12, _) => "Trop froid, fermentation lente garantie. Chauffe ou choisis une lager.",
            _ => "Conditions OK pour une fermentation ale (16-22°C idéal)."
        };

        return new FermentationForecast(avgMin, avgMax, recommendation);
    }
}

public record StockInfo(string Sku, string Name, double QuantityKg, bool LowStock, DateTime LastUpdated);
public record FermentationForecast(double AvgMinC, double AvgMaxC, string Recommendation);

internal record OpenMeteoResponse(DailyData Daily);
internal record DailyData(
    [property: System.Text.Json.Serialization.JsonPropertyName("temperature_2m_max")] double[] TemperatureMax,
    [property: System.Text.Json.Serialization.JsonPropertyName("temperature_2m_min")] double[] TemperatureMin);
