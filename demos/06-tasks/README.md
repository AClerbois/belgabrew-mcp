# Démo 6 — MCP Tasks : Long-Running Operations

> **Statut** : Expérimental dans la spec MCP (2025-11-25) — ✅ Supporté par le SDK .NET dès v0.7, stable en v1.2.0

## Concept

Les MCP Tasks permettent le pattern **"call-now, fetch-later"** pour les opérations longues :

```
Client → CallToolAsync(Task = {...}) → Serveur retourne TaskId (status: working)
Client → se déconnecte (optionnel)
Client → PollTaskUntilCompleteAsync(taskId) → résultat
```

Sans Tasks, un tool long ferait timeout côté LLM. Avec Tasks, le client reçoit un ID immédiatement et peut poller.

## Lifecycle

```
working → completed    ✅
        → failed       ❌
        → cancelled    🚫
        → input_required → working → completed   (si elicitation)
```

## Configuration

```csharp
// Program.cs
builder.Services.AddMcpServer(options =>
{
    options.TaskStore = new InMemoryMcpTaskStore();
    // options.SendTaskStatusNotifications = true; // SSE notifications
})
.WithHttpTransport(o => o.Stateless = true)
.WithTools<BrewingTaskTools>();
```

> **⚠️ Production** : `InMemoryMcpTaskStore` perd ses données au redémarrage.  
> Implémentez `IMcpTaskStore` avec un backend durable (SQL Server, Redis, Azure Blob…).

## Deux patterns

### 1. Automatic (recommandé pour la plupart des cas)

Tout tool `async Task<T>` supporte automatiquement le task-augmented call quand un `TaskStore` est configuré.

```csharp
[McpServerTool, Description("Analyse longue...")]
public async Task<FermentationAnalysis> AnalyseFermentationBatchAsync(
    bool includeRecent = false,
    CancellationToken ct = default)
{
    await Task.Delay(TimeSpan.FromSeconds(30), ct); // simulation
    return new FermentationAnalysis(...);
}
```

Le client passe `Task = new McpTaskMetadata { TimeToLive = TimeSpan.FromHours(1) }` → le SDK gère le reste.

### 2. Explicit (contrôle total)

Injectez `IMcpTaskStore` pour créer le task manuellement et lancer un fire-and-forget :

```csharp
[McpServerTool, Description("Lance la génération en arrière-plan")]
public async Task<McpTask> GenerateWeeklyReportAsync(
    IMcpTaskStore taskStore,
    RequestContext<CallToolRequestParams> context,
    string lang = "fr",
    CancellationToken ct = default)
{
    var task = await taskStore.CreateTaskAsync(
        new McpTaskMetadata { TimeToLive = TimeSpan.FromHours(2) },
        context.JsonRpcRequest.Id!,
        context.JsonRpcRequest,
        context.Server.SessionId,
        ct);

    _ = Task.Run(async () => {
        // ... travail long ...
        await taskStore.StoreTaskResultAsync(task.TaskId, McpTaskStatus.Completed, result, sessionId);
    }, CancellationToken.None);

    return task; // ← ID retourné immédiatement
}
```

## Appel côté client

```csharp
// Appel avec task metadata
var result = await client.CallToolAsync(new CallToolRequestParams
{
    Name = "generate_weekly_report_async",
    Task = new McpTaskMetadata { TimeToLive = TimeSpan.FromHours(1) }
});

// Option A : polling bloquant
var done = await client.PollTaskUntilCompleteAsync(result.Task!.TaskId);

// Option B : polling manuel
McpTask? taskStatus;
do {
    await Task.Delay(2000);
    taskStatus = await client.GetTaskAsync(result.Task!.TaskId);
} while (taskStatus?.Status == McpTaskStatus.Working);
```

## Fichiers

| Fichier | Description |
|---|---|
| `BrewingTaskTools.cs` | Deux tools : automatic (analyse) + explicit (rapport) |

## Lien avec les autres démos

| Démo | Primitive | Cas d'usage BelgaBrew |
|---|---|---|
| 1 | Tool | `check_ingredient_stock`, météo |
| 2 | Resource + Prompt | Recettes, rapport hebdo (synchrone) |
| 3 | Elicitation | Confirmation brassin |
| 4 | MCP App | Dashboard inventaire interactif |
| 5 | Copilot Studio | Intégration enterprise |
| **6** | **Tasks** | **Analyse fermentation longue, rapport batch** |
