# Démo 4 — MCP App : widget UI interactif

## Objectif pédagogique

Montrer qu'un serveur MCP peut pousser de l'HTML interactif directement dans le client, dans un iframe sandboxé.

## Concepts illustrés

- Resource avec `MimeType = "text/html;profile=mcp-app"` et URI `ui://`
- Communication bidirectionnelle via `postMessage` JSON-RPC
- Tool qui référence une UI resource via `[McpToolMeta]`
- Sandbox sécurisé — pas d'accès DOM du client

## Architecture MCP Apps

```
┌─────────────────────────────────┐
│  Client (VS Code / Claude)      │
│  ┌─────────────────────────┐   │
│  │  iframe sandboxé        │   │
│  │  (votre HTML/JS)        │◄──┼──── postMessage JSON-RPC
│  └─────────────────────────┘   │
└─────────────────────────────────┘
           │ calls tools/call
           ▼
┌─────────────────────────────────┐
│  Votre MCP Server (.NET)        │
│  check_ingredient_stock(sku)    │
└─────────────────────────────────┘
```

## Clients qui supportent MCP Apps (avril 2026)

| Client | Support |
|--------|---------|
| VS Code GitHub Copilot | ✅ |
| Claude Desktop | ✅ |
| ChatGPT | ✅ |
| Goose | ✅ |
| Copilot Studio | ❌ Pas encore |

## Comment tester

Dans VS Code Copilot agent mode ou Claude Desktop :
> "Montre-moi le dashboard inventaire"

Le widget HTML s'affiche dans la conversation avec les données en temps réel.

## Fichier principal

`demos/04-mcp-app/DashboardUiResource.cs`
