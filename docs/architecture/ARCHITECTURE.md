# Architecture

## Current architecture

```text
src/
├── TidyTop.App/
│   ├── Converters/       # Avalonia value converters
│   ├── Services/         # App composition and DI registration
│   ├── ViewModels/       # ReactiveUI view models
│   └── Views/            # Avalonia windows
└── TidyTop.Core/
    ├── Models/           # Domain models
    └── Services/         # Core services and interfaces
```

## Why the Data project was removed

The old `TidyTop.Data` project was empty. Empty projects add noise and make the architecture look more mature than it is.

Add a data project later only when there is a clear persistence responsibility, such as:

- SQLite storage.
- layout repository abstraction.
- migrations.
- import/export format handling.

For now, layout JSON persistence can live behind an interface in `TidyTop.Core.Services` until it grows.

## Domain language

Use these terms consistently:

| Term | Meaning |
| --- | --- |
| DesktopIcon | A file, shortcut, folder, or URL entry discovered from the desktop. |
| SmartBox | A visual container that groups desktop icons. |
| DesktopLayout | A saved arrangement of SmartBoxes and unboxed desktop icons. |
| ApplicationCategory | Starter categorization rules used for first-run grouping. |

Avoid `Fence` naming in new code.

## Layer responsibilities

### TidyTop.App

Responsible for:

- Windows and Avalonia UI.
- View models.
- User interaction.
- Rendering desktop items and SmartBoxes.
- Dialogs/settings screens.

Not responsible for:

- Domain rules.
- Layout persistence rules.
- Desktop scan logic.

### TidyTop.Core

Responsible for:

- Domain models.
- Desktop scan abstraction.
- SmartBox management.
- Layout management.
- Settings model and persistence.

Not responsible for:

- Avalonia controls.
- UI colors/layout details beyond serializable settings.
- Installer behavior.

## Windows-first technical decision

Desktop integration is OS-specific. For v0.1, treat Windows as the only supported runtime.

Future platform support should be introduced through interfaces such as:

```text
IDesktopItemProvider
IDesktopPositionService
IGlobalHotkeyService
IStartupRegistrationService
```

Then add platform implementations:

```text
TidyTop.Platform.Windows
TidyTop.Platform.Mac
TidyTop.Platform.Linux
```

Do not claim cross-platform support until these implementations exist and pass manual tests.

## Persistence direction

Start with simple JSON files under the user app data folder:

```text
%APPDATA%/TidyTop/settings.json
%APPDATA%/TidyTop/layouts/default.json
%APPDATA%/TidyTop/layouts/{layout-id}.json
```

Use SQLite only if JSON becomes painful because of search, history, migrations, or large data.

## UI direction

The UI should stay calm and practical:

- Dark translucent boxes.
- Clear title and count.
- Compact icon grid.
- Minimal animations.
- Keyboard shortcuts only after the core flow is stable.

## Testing strategy

Priority order:

1. Domain model tests.
2. SmartBox service tests.
3. Layout persistence tests.
4. Categorization tests.
5. Desktop scan tests with a fake folder provider.
6. UI smoke tests where practical.

Do not rely only on manual testing for layout save/restore.
