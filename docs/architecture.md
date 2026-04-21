# Architecture BelgaBrew MCP

## Vue d'ensemble

```
┌─────────────────────────────────────────────────────────────────┐
│                    MCP CLIENTS                                  │
│                                                                 │
│  VS Code Copilot  │  Claude Desktop  │  Copilot Studio         │
└────────────────────────────┬────────────────────────────────────┘
                             │ JSON-RPC 2.0 / Streamable HTTP
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│              BelgaBrew MCP Server (.NET 9)                      │
│                                                                 │
│  Tools                  Resources              Prompts          │
│  ├── check_ingredient   ├── brewery://recipes  └── weekly_      │
│  │   _stock             └── brewery://recipes      production_  │
│  ├── get_fermentation       /{recipeId}            report       │
│  │   _forecast                                                  │
│  ├── launch_brew        MCP Apps                                │
│  │   (+ elicitation)   └── ui://belgabrew/                     │
│  └── show_inventory         inventory-dashboard                 │
│      _dashboard                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Transports

- **Local dev** : stdio ou Streamable HTTP `http://localhost:5000/mcp`
- **Prod (Copilot Studio)** : Streamable HTTP HTTPS avec OAuth 2.0

## Stack technique

- .NET 9 / ASP.NET Core
- `ModelContextProtocol` 1.2.0 (Microsoft + Anthropic)
- `ModelContextProtocol.AspNetCore` 1.2.0
- Azure Container Apps (prod)
- Entra ID OAuth 2.0 (auth)
