using System.ComponentModel;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace BelgaBrew.Mcp.Tools;

/// <summary>
/// DEMO 3 — Elicitation : Human-in-the-loop
/// 
/// L'elicitation permet au SERVEUR MCP d'interrompre le flow
/// pour demander une confirmation structurée à l'HUMAIN (pas au LLM).
/// 
/// Pattern :
/// 1. Vérifier ClientCapabilities.Elicitation (pas tous les clients supportent)
/// 2. Appeler server.ElicitAsync() avec un schema JSON
/// 3. Traiter l'action retournée : "accept", "cancel", "decline"
/// </summary>
[McpServerToolType]
public class BrewingTools
{
    private readonly IBrewingService _brewing;

    public BrewingTools(IBrewingService brewing) => _brewing = brewing;

    [McpServerTool(Name = "launch_brew")]
    [Description("Lance un brassin pour une recette donnée. Opération coûteuse, confirmation humaine requise.")]
    public async Task<string> LaunchBrewAsync(
        IMcpServer server,          // SDK injecte le server context automatiquement
        [Description("Identifiant de la recette")] string recipeId,
        [Description("Volume en litres")] int volumeLiters,
        CancellationToken ct)
    {
        // ÉTAPE 1 : Vérifier que le client supporte l'elicitation
        // Toujours faire ce check — fallback propre sinon
        if (server.ClientCapabilities?.Elicitation is null)
        {
            return "⚠️ Ce client ne supporte pas l'elicitation. " +
                   "Brassin non lancé par sécurité — utilisez un client compatible (VS Code Copilot, Claude Desktop).";
        }

        // ÉTAPE 2 : Construire le schema de la demande
        // Le client peut afficher un formulaire propre depuis ce schema
        var elicitResult = await server.ElicitAsync(
            new ElicitRequestParams
            {
                Message = $"⚠️ Confirmer le lancement d'un brassin de {volumeLiters}L de « {recipeId} » ? " +
                          $"Coût estimé : {volumeLiters * 0.8:F0}€. Durée : 3 semaines.",
                RequestedSchema = new ElicitRequestParams.RequestSchema
                {
                    Properties = new Dictionary<string, ElicitRequestParams.PrimitiveSchemaDefinition>
                    {
                        // BooleanSchema → checkbox
                        ["confirm"] = new ElicitRequestParams.BooleanSchema
                        {
                            Description = "Confirmez-vous le lancement ?"
                        },
                        // StringSchema → text input
                        ["responsible"] = new ElicitRequestParams.StringSchema
                        {
                            Description = "Nom du maître-brasseur responsable du brassin"
                        },
                        // UntitledSingleSelectEnumSchema → dropdown
                        ["priority"] = new ElicitRequestParams.UntitledSingleSelectEnumSchema
                        {
                            Description = "Priorité du brassin",
                            Enum = ["standard", "express", "evenement"]
                        }
                    }
                }
            }, ct);

        // ÉTAPE 3 : Traiter la réponse
        // Action peut être : "accept", "cancel" (dismiss), "decline" (refus explicite)
        if (elicitResult.Action != "accept")
            return "❌ Brassin annulé par l'opérateur.";

        var confirm = elicitResult.Content?["confirm"].GetBoolean() ?? false;
        if (!confirm)
            return "❌ Brassin refusé par l'opérateur.";

        var responsible = elicitResult.Content!["responsible"].GetString()!;
        var priority    = elicitResult.Content["priority"].GetString()!;

        var brewId = await _brewing.LaunchAsync(recipeId, volumeLiters, responsible, priority, ct);

        return $"✅ Brassin lancé. ID: {brewId}. Responsable: {responsible}. Priorité: {priority}.";
    }
}
