# Architecture

## Current architecture

```text
src/
├── TidyTop.App/
│   ├── Commands/        # UI command helpers
│   ├── Services/        # App composition and DI registration
│   ├── ViewModels/      # UI state and command orchestration
│   └── Views/           # Avalonia XAML windows
└── TidyTop.Core/
    ├── Models/          # Domain models and runtime projections
    └── Services/        # Scanning, reconciliation, persistence, workspace orchestration
```

## Domain model

| Model | Purpose |
| --- | --- |
| `DesktopItem` | One real file/folder/shortcut discovered from desktop folders. |
| `SmartBox` | A persisted container that stores assigned item paths and optional matching rules. |
| `SmartBoxRule` | A simple rule: extension, name contains, path contains, or item type. |
| `DesktopLayout` | The persisted layout containing SmartBoxes. |
| `DesktopWorkspace` | Runtime projection of layout + current desktop scan. |
| `SmartBoxSnapshot` | One SmartBox plus the live `DesktopItem` objects it currently contains. |
| `AppSettings` | Settings independent from a specific layout. |

## Important foundation decision

SmartBoxes persist **normalized item paths**, not copied desktop item objects.

Reason: desktop files change. A saved layout should remember assignment identity, then reconcile against the latest scan. This avoids stale duplicated items and makes deleted/new desktop files easier to handle.

## Core services

| Service | Responsibility |
| --- | --- |
| `DesktopScanner` | Best-effort scan of user/public desktop folders. |
| `DefaultSmartBoxFactory` | Creates first-run system SmartBoxes. |
| `LayoutReconciler` | Cleans assignments, applies rules, and fills catch-all box. |
| `JsonLayoutStore` | Saves/loads layout JSON atomically. |
| `JsonAppSettingsStore` | Saves/loads settings JSON. |
| `DesktopWorkspaceService` | High-level scan → reconcile → save flow used by the UI. |

## App layer

`TidyTop.App` should stay thin:

- create the DI container,
- bind views to view models,
- display SmartBoxes and desktop item rows,
- send user commands to core services.

It should not own categorization, persistence, or scan rules.

## Persistence

Current files:

```text
%APPDATA%/TidyTop/layout.json
%APPDATA%/TidyTop/settings.json
```

This is enough for v0.1. SQLite should wait until there is a real need for migration history, multiple layouts, search, or analytics.

## Windows-first decision

The app project targets `net8.0-windows`. The Core project targets `net8.0` and avoids UI/Desktop API dependencies where possible.

Future platform-specific work should be isolated behind interfaces:

```text
IDesktopPositionService
IGlobalHotkeyService
ITrayService
IStartupRegistrationService
```

Do not claim cross-platform desktop organization until platform implementations actually exist.

## Testing strategy

Priority order:

1. domain model tests,
2. reconciliation tests,
3. persistence tests,
4. scanner tests with temporary directories or fake providers,
5. view-model tests,
6. manual Windows smoke tests for desktop behavior.
