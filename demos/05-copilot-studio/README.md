# Démo 5 — Copilot Studio en entreprise

## Objectif pédagogique

Passer d'un dev tool local à une prod enterprise avec gouvernance Power Platform.

## Prérequis (à préparer avant le talk)

1. Déployer le serveur BelgaBrew sur **Azure Container Apps** (cf. `deploy/bicep/`)
2. Configurer **Entra ID** OAuth 2.0
3. Note l'URL : `https://belgabrew.azurecontainerapps.io/mcp`
4. Créer un agent "BelgaBrew Assistant" dans Copilot Studio

## Étapes de la démo

### 1. Vérifier le serveur

```
GET https://belgabrew.azurecontainerapps.io/health
→ { "status": "ok", "server": "BelgaBrew MCP" }
```

### 2. Connecter dans Copilot Studio

```
Tools → Add a tool → New tool → Model Context Protocol
  Server name        : BelgaBrew MCP
  Server description : Gestion de la micro-brasserie BelgaBrew
  Server URL         : https://belgabrew.azurecontainerapps.io/mcp
  Authentication     : OAuth 2.0 (ou API key pour la démo)
```

### 3. Tester avec l'agent

- `"Quel est le stock de houblon Saaz ?"` → appelle `check_ingredient_stock`
- `"Quelle est la prévision fermentation pour Charleroi ?"` → appelle `get_fermentation_forecast`
- `"Montre-moi la recette de la triple 2026"` → lit `brewery://recipes/triple-abbaye-2026`

### 4. Gouvernance à montrer

- **Toggle par tool** : désactiver `launch_brew` pour les agents non-admin
- **Connection management** : qui a le droit d'utiliser ce MCP
- **DLP policies** : via Power Platform admin center
- **Audit logs** : toutes les calls sont tracées

## Ce que Copilot Studio supporte (avril 2026)

| Primitive MCP | Support |
|---------------|---------|
| Tools | ✅ |
| Resources | ✅ |
| Prompts | ❌ Pas encore |
| Elicitation | ❌ Pas encore |
| MCP Apps | ❌ Pas encore |

## Auth en prod : règles impératives

- ✅ Entra ID (OAuth 2.0 DCR) pour multi-tenant
- ✅ OAuth 2.0 client credentials pour service-to-service
- ❌ Clé API en prod — interdit
- ❌ Endpoint MCP public non authentifié — suicide

## Punchline

> "Un même serveur MCP = utilisable dans Copilot Studio ET VS Code ET Claude Desktop.
> Vous écrivez une fois. Votre architecte d'entreprise va adorer."
