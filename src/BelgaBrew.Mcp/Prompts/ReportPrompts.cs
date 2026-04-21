using System.ComponentModel;
using BelgaBrew.Mcp.Services;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace BelgaBrew.Mcp.Prompts;

[McpServerPromptType]
public class ReportPrompts(IBrewingService brewing, IInventoryService inventory)
{
    [McpServerPrompt(Name = "weekly_production_report")]
    [Description("Génère un rapport hebdomadaire de production : brassins actifs + alertes stock.")]
    public async Task<IEnumerable<ChatMessage>> GetWeeklyReportPrompt(
        [Description("Langue du rapport (fr, en, nl)")] string lang = "fr",
        CancellationToken ct = default)
    {
        var activeBrews = await brewing.GetActiveBrewsAsync(ct);
        var lowStock    = await inventory.GetLowStockAsync(ct);

        var data = $"""
        Active brews: {System.Text.Json.JsonSerializer.Serialize(activeBrews)}
        Low stock items: {System.Text.Json.JsonSerializer.Serialize(lowStock)}
        """;

        return
        [
            new ChatMessage(ChatRole.System,
                $"You are the BelgaBrew production manager. Always answer in {lang}."),

            new ChatMessage(ChatRole.User, $"""
                Génère le rapport de production hebdomadaire pour BelgaBrew.

                Données temps réel :
                {data}

                Structure attendue :
                1. Résumé exécutif (3 lignes max)
                2. Brassins en cours et leur statut
                3. Alertes stock à traiter cette semaine
                4. Recommandations pour la semaine prochaine
                """)
        ];
    }
}
