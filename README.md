# 🍺 BelgaBrew MCP Server

> Demo project for the talk **"Construire son propre serveur MCP en .NET : de zéro à une démo multi-tools"**  
> AICD Paris — Adrien Clerbois

## Quick Start

```bash
# Clone the repo
git clone https://github.com/AClerbois/belgabrew-mcp
cd belgabrew-mcp

# Run the MCP server
cd src/BelgaBrew.Mcp
dotnet run

# Open MCP Inspector (in another terminal)
# Option A — web UI (recommended): open http://localhost:6274 then set transport to
#   "Streamable HTTP" and URL to http://localhost:5050/mcp
npx @modelcontextprotocol/inspector

# Option B — CLI direct connect
npx @modelcontextprotocol/inspector --transport streamable-http --url http://localhost:5050/mcp
```

## Project Structure

```
belgabrew-mcp/
├── src/
│   └── BelgaBrew.Mcp/           # The MCP server
│       ├── Program.cs
│       ├── Tools/
│       │   ├── BreweryTools.cs  # Demo 1 — Tool + Open-Meteo API
│       │   └── BrewingTools.cs  # Demo 3 — Elicitation
│       ├── Resources/
│       │   ├── RecipeResources.cs        # Demo 2 — Resources
│       │   └── DashboardUiResource.cs    # Demo 4 — MCP Apps
│       ├── Prompts/
│       │   └── ReportPrompts.cs  # Demo 2 — Prompts
│       └── Services/             # Mock services
├── demos/
│   ├── 01-tool-api/              # Demo 1 — Tool calling an external API
│   ├── 02-resources-prompts/     # Demo 2 — Resources & Prompts
│   ├── 03-elicitation/           # Demo 3 — Human-in-the-loop
│   ├── 04-mcp-app/               # Demo 4 — Interactive UI widget
│   └── 05-copilot-studio/        # Demo 5 — Copilot Studio enterprise
├── deploy/
│   └── bicep/                    # Azure Container Apps deployment
└── docs/
    ├── architecture.md
    └── step-by-step.md
```

## Demos

| # | Title | Primitive | Highlights |
|---|-------|-----------|------------|
| 1 | Tool + Open-Meteo API | `[McpServerTool]` | External HTTP, schema auto-mapping |
| 2 | Resources + Prompts | `[McpServerResource]` `[McpServerPrompt]` | URI templates, prompt macros |
| 3 | Elicitation | `server.ElicitAsync()` | Human-in-the-loop, structured form |
| 4 | MCP App (UI widget) | `text/html;profile=mcp-app` | Interactive HTML in the chat |
| 5 | Copilot Studio | Streamable HTTP + Entra ID | Enterprise governance |

## Resources

- [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)
- [MCP Specification](https://modelcontextprotocol.io/specification)
- [Copilot Studio MCP docs](https://learn.microsoft.com/microsoft-copilot-studio/mcp-add-existing-server-to-agent)
