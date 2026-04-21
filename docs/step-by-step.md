# Guide pas à pas — Construire son MCP server .NET

## 1. Créer le projet

```bash
dotnet new web -n MyMcp
cd MyMcp
dotnet add package ModelContextProtocol
dotnet add package ModelContextProtocol.AspNetCore
```

## 2. Configurer Program.cs

```csharp
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly()
    .WithResourcesFromAssembly()
    .WithPromptsFromAssembly();

app.MapMcp();
```

## 3. Créer un Tool

```csharp
[McpServerToolType]
public class MyTools
{
    [McpServerTool(Name = "my_tool")]
    [Description("Ce que fait mon tool — lu par le LLM.")]
    public string MyTool([Description("Description du param")] string input)
        => $"Result: {input}";
}
```

## 4. Créer une Resource

```csharp
[McpServerResourceType]
public class MyResources
{
    [McpServerResource(Uri = "myapp://data", MimeType = "application/json")]
    public string GetData() => JsonSerializer.Serialize(new { ok = true });
}
```

## 5. Créer un Prompt

```csharp
[McpServerPromptType]
public class MyPrompts
{
    [McpServerPrompt(Name = "my_prompt")]
    public IEnumerable<ChatMessage> GetPrompt(string context)
    {
        yield return new ChatMessage(ChatRole.System, "Tu es un assistant.");
        yield return new ChatMessage(ChatRole.User, $"Aide-moi avec : {context}");
    }
}
```

## 6. Tester avec MCP Inspector

```bash
npx @modelcontextprotocol/inspector http://localhost:5000/mcp
```

## 7. Déployer sur Azure Container Apps

```bash
cd deploy/bicep
az deployment group create -g my-rg -f main.bicep
```
