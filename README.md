# TidyTop

TidyTop is a Windows-first desktop organizer built with .NET 8 and Avalonia. Its first useful goal is deliberately small: scan the Windows desktop, group real desktop items into SmartBoxes, persist the layout, and restore it safely on restart.

TidyTop is not positioned as a clone. The product direction is:

> A lightweight desktop organization layer for people who want a cleaner desktop without replacing Windows Explorer.

## Current foundation

This rewrite makes the repository match the real MVP scope:

- real domain model: `DesktopItem`, `SmartBox`, `DesktopLayout`, `DesktopWorkspace`
- stable path-based item identity
- desktop scanner for user and public desktop folders
- rule-based default SmartBoxes
- catch-all `Other / Unboxed` SmartBox
- layout reconciliation for new/deleted desktop items
- JSON layout persistence under `%APPDATA%/TidyTop/layout.json`
- JSON settings store under `%APPDATA%/TidyTop/settings.json`
- Avalonia UI bound to view models, not manual code-behind rendering
- Windows desktop overlay host: borderless transparent desktop surface attached to WorkerW/Progman when possible
- canvas-based SmartBox placement using saved X/Y/width/height values
- tests for rules, SmartBoxes, layout cloning, reconciliation, layout JSON round trip, and main view-model loading

## MVP product loop

The v0.1 loop is:

1. Scan desktop items.
2. Display them in SmartBoxes.
3. Create manual SmartBoxes.
4. Move items between SmartBoxes.
5. Save layout automatically.
6. Restore layout on restart.
7. Add quick hide/show.

The current rewrite completes the foundation for steps 1, 2, 3, 5, and 6 at the service/model level, and now includes the first Windows desktop-overlay pass. Manual drag/drop movement, visual move/resize, and native icon handling are the next major pieces.

## Tech stack

- .NET 8
- Avalonia UI
- ReactiveUI base objects
- Microsoft dependency injection
- xUnit
- Windows-first runtime target for the app

## Repository structure

```text
TidyTop/
├── assets/                    # Brand assets
├── docs/                      # Product, architecture, roadmap, and workflow docs
├── src/
│   ├── TidyTop.App/           # Avalonia UI, commands, view models, composition
│   └── TidyTop.Core/          # Domain models, scanning, reconciliation, persistence
├── tests/
│   ├── TidyTop.App.Tests/     # View-model tests
│   └── TidyTop.Core.Tests/    # Domain and service tests
├── Directory.Build.props
├── TidyTop.sln
└── README.md
```

## Build

Prerequisite: .NET 8 SDK.

```bash
dotnet restore
dotnet build
```

## Run

```bash
dotnet run --project src/TidyTop.App/TidyTop.App.csproj
```

On Windows, the app now attempts to appear directly on the desktop as a transparent overlay. If Explorer blocks the desktop host attachment, the app should still open as a borderless transparent window instead of crashing.

## Test

```bash
dotnet test
```

## Development rule

Do not mark a feature as complete unless it is wired into the app and covered by tests or a written manual verification note. Keep `docs/STATUS.md` and `docs/roadmap/TASKS.md` updated after each completed task.
