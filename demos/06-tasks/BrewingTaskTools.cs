using System.ComponentModel;
using System.Text.Json;
using BelgaBrew.Mcp.Services;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace BelgaBrew.Mcp.Tools;

/// <summary>
/// DEMO 6 — MCP Tasks : Long-Running Operations
///
/// Pattern "call-now, fetch-later" :
///   1. Le client appelle le tool avec Task = new McpTaskMetadata { ... }
///   2. Le serveur retourne immédiatement un TaskId (status = working)
///   3. Le client peut se déconnecter et revenir plus tard
///   4. Le client poll avec client.GetTaskAsync(taskId) ou PollTaskUntilCompleteAsync()
///
/// Configuration dans Program.cs :
///   builder.Services.AddMcpServer(options => {
///       options.TaskStore = new InMemoryMcpTaskStore();
///   });
///
/// ⚠️  Expérimental dans la spec MCP (2025-11-25)
/// ✅  Supporté par le SDK .NET dès v0.7 — stable en v1.2.0
///
/// Lifecycle :
///   working → completed / failed / cancelled
///   working → input_required (si elicitation) → working → completed
/// </summary>
[McpServerToolType]
public class BrewingTaskTools(IBrewingService brewing, IInventoryService inventory)
{
    /// <summary>
    /// Exemple 1 — Automatic task support.
    ///
    /// Tout tool qui retourne Task{T} ou ValueTask{T} supporte automatiquement
    /// le task-augmented call (TaskSupport = Optional par défaut).
    /// Le client décide d'utiliser ou non les tasks en passant Task = new McpTaskMetadata().
    /// </summary>
    [McpServerTool(Name = "analyse_fermentation_batch")]
    [Description(
        "Analyse complète de l'ensemble des brassins actifs : " +
        "qualité de fermentation, prévisions de rendement, alertes. " +
        "Opération longue (20-120 secondes selon le nombre de brassins).")]
    public async Task<FermentationAnalysis> AnalyseFermentationBatchAsync(
        [Description("Inclure les brassins terminés des 30 derniers jours")] bool includeRecent = false,
        CancellationToken ct = default)
    {
        // Simule une analyse longue — en production : appel ML, calculs de densité, etc.
        var activeBrews = await brewing.GetActiveBrewsAsync(ct);

        await Task.Delay(TimeSpan.FromSeconds(5), ct); // Simulation traitement long

        var alerts = activeBrews
            .Where(b => b.Status == "Fermentation" && DateTime.UtcNow - b.StartedAt > TimeSpan.FromDays(14))
            .Select(b => $"⚠️ Brassin {b.BrewId} ({b.RecipeId}) : fermentation anormalement longue")
            .ToList();

        return new FermentationAnalysis(
            AnalysedAt: DateTime.UtcNow,
            TotalBrewing: activeBrews.Count,
            Alerts: alerts,
            Recommendation: alerts.Count > 0
                ? "Vérifier la température et la levure des brassins en alerte."
                : "Tous les brassins progressent normalement."
        );
    }

    /// <summary>
    /// Exemple 2 — Explicit task creation with IMcpTaskStore.
    ///
    /// Pour un contrôle total sur le cycle de vie, le tool injecte IMcpTaskStore,
    /// crée le task manuellement, lance le travail en arrière-plan (fire-and-forget),
    /// et retourne McpTask immédiatement.
    ///
    /// Utile quand :
    ///   - On veut soumettre à un job queue externe (Azure Service Bus, Hangfire…)
    ///   - On a besoin de durabilité (file-based store, SQL, Redis)
    ///   - On veut mettre à jour le status manuellement depuis un autre process
    /// </summary>
    [McpServerTool(Name = "generate_weekly_report_async")]
    [Description(
        "Lance la génération du rapport hebdomadaire de production en arrière-plan. " +
        "Retourne un taskId immédiatement. Utilisez le polling pour récupérer le résultat.")]
    public async Task<McpTask> GenerateWeeklyReportAsync(
        IMcpTaskStore taskStore,
        RequestContext<CallToolRequestParams> context,
        [Description("Langue du rapport (fr, en, nl)")] string lang = "fr",
        CancellationToken ct = default)
    {
        // Créer le task dans le store → retour immédiat au client
        var task = await taskStore.CreateTaskAsync(
            new McpTaskMetadata { TimeToLive = TimeSpan.FromHours(2) },
            context.JsonRpcRequest.Id!,
            context.JsonRpcRequest,
            context.Server.SessionId,
            ct);

        var sessionId = context.Server.SessionId;

        // Lancer le travail en arrière-plan (fire-and-forget)
        _ = Task.Run(async () =>
        {
            try
            {
                // Simulation du travail long : collecte des données, génération
                await Task.Delay(TimeSpan.FromSeconds(8));

                var activeBrews = await brewing.GetActiveBrewsAsync(CancellationToken.None);
                var lowStock    = await inventory.GetLowStockAsync(CancellationToken.None);

                var report = $"""
                    === Rapport Hebdomadaire BelgaBrew ({lang.ToUpper()}) ===
                    Généré le : {DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC

                    BRASSINS EN COURS : {activeBrews.Count}
                    {string.Join("\n", activeBrews.Select(b => $"  • {b.BrewId} — {b.RecipeId} ({b.Status}) — Responsable: {b.Responsible}"))}

                    ALERTES STOCK : {lowStock.Count} article(s) en rupture
                    {string.Join("\n", lowStock.Select(i => $"  ⚠️ {i.Name} : {i.QuantityKg:F1} kg (seuil: {i.ReorderThresholdKg:F1} kg)"))}

                    RECOMMANDATIONS :
                    {(lowStock.Count > 0 ? "→ Commander les ingrédients en rupture avant le prochain brassin." : "→ Stock nominal. Aucune action requise.")}
                    """;

                // Stocker le résultat et passer le status à "completed"
                await taskStore.StoreTaskResultAsync(
                    task.TaskId,
                    McpTaskStatus.Completed,
                    JsonSerializer.SerializeToElement(new CallToolResult
                    {
                        Content = [new TextContentBlock { Text = report }]
                    }),
                    sessionId);
            }
            catch (Exception ex)
            {
                // En cas d'erreur → status "failed"
                await taskStore.StoreTaskResultAsync(
                    task.TaskId,
                    McpTaskStatus.Failed,
                    JsonSerializer.SerializeToElement(new CallToolResult
                    {
                        Content = [new TextContentBlock { Text = $"Erreur lors de la génération : {ex.Message}" }],
                        IsError = true
                    }),
                    sessionId);
            }
        }, CancellationToken.None);

        // Retour immédiat — le client peut se déconnecter et revenir plus tard
        return task;
    }
}

public record FermentationAnalysis(
    DateTime AnalysedAt,
    int TotalBrewing,
    List<string> Alerts,
    string Recommendation);
