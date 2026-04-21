using System.ComponentModel;
using BelgaBrew.Mcp.Services;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace BelgaBrew.Mcp.Tools;

[McpServerToolType]
public class BrewingTools(IBrewingService brewing)
{
    [McpServerTool(Name = "launch_brew")]
    [Description("Lance un brassin pour une recette donnée. Opération coûteuse, confirmation humaine requise.")]
    public async Task<string> LaunchBrewAsync(
        McpServer server,
        [Description("Identifiant de la recette")] string recipeId,
        [Description("Volume en litres")] int volumeLiters,
        CancellationToken ct)
    {
        if (server.ClientCapabilities?.Elicitation is null)
        {
            return "⚠️ Ce client ne supporte pas l'elicitation. " +
                   "Brassin non lancé par sécurité — utilisez un client compatible (VS Code Copilot, Claude Desktop).";
        }

        var elicitResult = await server.ElicitAsync(
            new ElicitRequestParams
            {
                Message = $"⚠️ Confirmer le lancement d'un brassin de {volumeLiters}L de « {recipeId} » ? " +
                          $"Coût estimé : {volumeLiters * 0.8:F0}€. Durée : 3 semaines.",
                RequestedSchema = new ElicitRequestParams.RequestSchema
                {
                    Properties = new Dictionary<string, ElicitRequestParams.PrimitiveSchemaDefinition>
                    {
                        ["confirm"] = new ElicitRequestParams.BooleanSchema
                        {
                            Description = "Confirmez-vous le lancement ?"
                        },
                        ["responsible"] = new ElicitRequestParams.StringSchema
                        {
                            Description = "Nom du maître-brasseur responsable du brassin"
                        },
                        ["priority"] = new ElicitRequestParams.UntitledSingleSelectEnumSchema
                        {
                            Description = "Priorité du brassin",
                            Enum = ["standard", "express", "evenement"]
                        }
                    }
                }
            }, ct);

        if (elicitResult.Action != "accept")
            return "❌ Brassin annulé par l'opérateur.";

        var confirm = elicitResult.Content?["confirm"].GetBoolean() ?? false;
        if (!confirm)
            return "❌ Brassin refusé par l'opérateur.";

        var responsible = elicitResult.Content!["responsible"].GetString()!;
        var priority    = elicitResult.Content["priority"].GetString()!;

        var brewId = await brewing.LaunchAsync(recipeId, volumeLiters, responsible, priority, ct);
        return $"✅ Brassin lancé. ID: {brewId}. Responsable: {responsible}. Priorité: {priority}.";
    }
}
