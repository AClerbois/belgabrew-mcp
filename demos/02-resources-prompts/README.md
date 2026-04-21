# Démo 2 — Resources + Prompts

## Objectif pédagogique

Montrer qu'MCP ≠ seulement tool calling. Illustrer la frontière **Tool vs Resource**, et les **Prompts** comme macros réutilisables.

## Concepts illustrés

### Resources
- `[McpServerResourceType]` sur la classe
- `[McpServerResource(Uri = "...")]` — resource statique par URI
- `[McpServerResource(UriTemplate = ".../{ id}")]` — resource dynamique par URI template
- MimeType déclaré dans l'attribut
- La resource est **lue** par le LLM (contexte) — pas exécutée

### Prompts
- `[McpServerPromptType]` sur la classe
- `[McpServerPrompt(Name = "...")]` sur la méthode
- Retourne `IEnumerable<ChatMessage>` — messages système + utilisateur structurés
- Paramètres typés avec valeurs par défaut

## Règle du pouce : Tool vs Resource

| | Tool | Resource |
|-|------|----------|
| Effet ? | ✅ Fait quelque chose | ❌ Montre quelque chose |
| Exemple | `send_email` | `inventory.json` |
| Analogie SQL | Stored procedure | Vue (VIEW) |

## Comment tester

Dans l'inspecteur MCP :
1. Onglet **Resources** → lire `brewery://recipes`
2. Onglet **Resources** → lire `brewery://recipes/triple-abbaye-2026`
3. Onglet **Prompts** → invoquer `weekly_production_report` avec `weekNumber: 17`

## Fichiers

- `demos/02-resources-prompts/RecipeResources.cs`
- `demos/02-resources-prompts/ReportPrompts.cs`
