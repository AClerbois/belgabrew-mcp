# Démo 3 — Elicitation : Human-in-the-loop

## Objectif pédagogique

Montrer le **human-in-the-loop propre** via l'elicitation MCP. C'est LA primitive sous-utilisée.

## Concepts illustrés

- `server.ElicitAsync()` — suspension du tool pour demander une confirmation humaine
- `ClientCapabilities.Elicitation` — vérification si le client supporte l'elicitation
- Schema structuré : `BooleanSchema`, `StringSchema`, `UntitledSingleSelectEnumSchema`
- Gestion des cas `accept` / `cancel` / `decline`
- Fallback propre si le client ne supporte pas l'elicitation

## Schéma d'elicitation supporté

```
ElicitRequestParams.RequestSchema
├── BooleanSchema       → checkbox
├── StringSchema        → text input
├── NumberSchema        → number input
└── UntitledSingleSelectEnumSchema  → dropdown
```

## Clients qui supportent l'elicitation (avril 2026)

| Client | Support |
|--------|---------|
| VS Code GitHub Copilot | ✅ |
| Claude Desktop | ✅ |
| ChatGPT | 🟡 Partiel |
| Copilot Studio | ❌ Pas encore |

## Comment tester

Dans VS Code Copilot agent mode :
> "Lance 200 litres de triple Abbaye"

Un formulaire modal apparaît dans le chat avec :
- ☑ Confirmation
- 📝 Nom du maître-brasseur
- 📋 Priorité (standard / express / evenement)

## Sécurité : ce qu'on ne met PAS dans une elicitation

- ❌ Mots de passe
- ❌ Numéros de carte bancaire
- ❌ Données sensibles
- ✅ Pour les données sensibles → utiliser le `url` mode (redirection vers page externe sécurisée)

## Fichier principal

`demos/03-elicitation/BrewingTools.cs`
