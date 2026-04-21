using BelgaBrew.Mcp.Services;

var builder = WebApplication.CreateBuilder(args);

// Register mock services
builder.Services.AddSingleton<IInventoryService, MockInventoryService>();
builder.Services.AddSingleton<IRecipeRepository, MockRecipeRepository>();
builder.Services.AddSingleton<IBrewingService, MockBrewingService>();
builder.Services.AddHttpClient();

// Register MCP server with all primitives
builder.Services
    .AddMcpServer()
    .WithHttpTransport()           // Streamable HTTP — required for Copilot Studio
    .WithToolsFromAssembly()       // Scans [McpServerToolType] classes
    .WithResourcesFromAssembly()   // Scans [McpServerResourceType] classes
    .WithPromptsFromAssembly();    // Scans [McpServerPromptType] classes

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok", server = "BelgaBrew MCP" }));
app.MapMcp(); // Exposes /mcp endpoint

app.Run();
