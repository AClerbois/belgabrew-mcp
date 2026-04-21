using System.ComponentModel;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace BelgaBrew.Mcp.Prompts;

/// <summary>
/// DEMO 2 — Prompts MCP
/// Un Prompt MCP est une MACRO réutilisable :
///   - Paramétrable (arguments typés)
///   - Retourne une séquence de ChatMessage (System + User)
///   - Pensez-le comme une stored procedure pour vos prompts
///   - Garantit que toute l'équipe utilise le même prompt structuré
/// </summary>
[McpServerPromptType]
public class ReportPrompts
{
    [McpServerPrompt(Name = "weekly_production_report")]
    [Description("Génère un rapport de production hebdomadaire cohérent avec le format maison.")]
    public IEnumerable<ChatMessage> GenerateWeeklyReport(
        [Description("Numéro de la semaine ISO, ex: 17")] int weekNumber,
        [Description("Inclure les écarts avec la semaine précédente")] bool includeComparison = true)
    {
        // Message système : persona et contraintes
        yield return new ChatMessage(
            ChatRole.System,
            "Tu es l'assistant de production de BelgaBrew. Tu rédiges des rapports en français, " +
            "ton factuel, 200 mots max, avec une section 'points d'attention' en fin.");

        // Message utilisateur : instruction concrète qui pointe vers les Resources MCP
        yield return new ChatMessage(
            ChatRole.User,
            $"Génère le rapport de production pour la semaine {weekNumber} de 2026." +
            (includeComparison ? " Compare avec la semaine précédente." : "") +
            $" Utilise les ressources brewery://production/week-{weekNumber} et brewery://recipes.");
    }
}
