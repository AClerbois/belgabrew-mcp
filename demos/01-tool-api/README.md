# Démo 1 — Tool : appel à une API externe

## Objectif pédagogique

Montrer les attributs `[McpServerTool]`, le mapping automatique du schéma JSON, et un appel HTTP externe non trivial.

## Concepts illustrés

- `[McpServerToolType]` sur la classe
- `[McpServerTool(Name = "...")]` sur la méthode
- `[Description]` sur la méthode ET les paramètres — lu par le LLM pour décider d'appeler
- Injection de dépendances (`IHttpClientFactory`, services custom)
- Records sérialisés en JSON structured content automatiquement
- `CancellationToken` transmis par le SDK

## Fichier principal

`src/BelgaBrew.Mcp/Tools/BreweryTools.cs`

## Comment tester

```bash
# Terminal 1 — lancer le serveur
cd src/BelgaBrew.Mcp
dotnet run

# Terminal 2 — ouvrir l'inspecteur MCP
npx @modelcontextprotocol/inspector --transport streamable-http --url http://localhost:5050/mcp
```

Dans l'inspecteur :
1. Aller dans **Tools**
2. Cliquer `check_ingredient_stock`
3. Passer `{ "sku": "HOP-SAAZ-001" }`
4. Observer le JSON structuré retourné

Puis dans VS Code Copilot agent mode :
> "Quel est le stock du houblon Saaz ?"

## ⚠️ Commande inspector correcte

```bash
# ❌ NE PAS faire — passe l'URL comme commande stdio :
npx @modelcontextprotocol/inspector http://localhost:5050/mcp

# ✅ Faire — spécifier le transport HTTP explicitement :
npx @modelcontextprotocol/inspector --transport streamable-http --url http://localhost:5050/mcp

# ✅ Ou lancer le web UI et configurer manuellement :
npx @modelcontextprotocol/inspector
# Ouvre http://localhost:6274 → transport: Streamable HTTP → URL: http://localhost:5050/mcp
```

## Points clés à dire

- Les `[Description]` ne sont **pas cosmétiques** : c'est ce que le LLM lit pour décider d'appeler
- Les records sont sérialisés automatiquement — pas besoin de `JsonSerializer.Serialize()`
- Le serveur ne sait pas qui lui parle (Claude, Copilot, ChatGPT…) → c'est ça, un protocole
