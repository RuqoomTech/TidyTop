# TidyTop

TidyTop is a Windows-first desktop organizer built with .NET 8 and Avalonia. Its first useful goal is deliberately small: scan the Windows desktop, group real desktop items into SmartBoxes, persist the layout, and restore it safely on restart.

TidyTop is not positioned as a clone. The product direction is:

> A lightweight desktop organization layer for people who want a cleaner desktop without replacing Windows Explorer.

## Current foundation

The repository now matches the real MVP scope:

- real domain model: `DesktopItem`, `SmartBox`, `DesktopLayout`, `DesktopWorkspace`
- stable path-based item identity
- desktop scanner for user and public desktop folders
- rule-based default SmartBoxes
- catch-all `Other / Unboxed` SmartBox
- layout reconciliation for new/deleted desktop items
- atomic JSON layout persistence under `%APPDATA%/TidyTop/layout.json`
- atomic JSON settings persistence under `%APPDATA%/TidyTop/settings.json`
- `.bak` recovery for broken layout/settings files
- file logging under `%APPDATA%/TidyTop/logs/tidytop.log`
- Avalonia UI bound to view models, not manual code-behind rendering
- Windows desktop overlay host: borderless transparent desktop surface attached to WorkerW/Progman when possible
- canvas-based SmartBox placement using saved X/Y/width/height values
- visual SmartBox move/resize with autosaved geometry on pointer release
- double-click desktop items or press Open to launch files, folders, shortcuts, and URLs through the OS shell
- drag an item from one SmartBox and drop it onto another SmartBox to manually reassign it
- tray menu, quick hide/show, global hotkey, and safer native desktop icon controls
- emergency **Restore icons** action if Explorer's native icon view does not reappear immediately
- Settings & Diagnostics panel for feature flags, emergency actions, reset controls, folders, and live runtime state

## MVP product loop

The v0.1 loop is:

1. Scan desktop items.
2. Display them in SmartBoxes.
3. Create manual SmartBoxes.
4. Move items between SmartBoxes.
5. Save layout automatically.
6. Restore layout on restart.
7. Show/hide TidyTop quickly.
8. Safely hide/show native Windows desktop icons without touching files.

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
│   ├── TidyTop.App/           # Avalonia UI, commands, view models, desktop integration
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

On Windows, the app attempts to appear directly on the desktop as a transparent overlay. If Explorer blocks the desktop host attachment, the app should still open as a borderless transparent window instead of crashing.

## Test

```bash
dotnet test
```

## Desktop integration safety

TidyTop can hide/show Explorer's native desktop icon view, but it never deletes, moves, renames, or rewrites real Desktop files.

If Windows desktop icons do not reappear while TidyTop is running:

1. Press **Restore icons** in the toolbar.
2. Or use tray menu → **Restore Windows icons**.
3. If needed, exit TidyTop from the tray menu.
4. Open **Settings** → **Diagnostics** to inspect runtime state.
5. Check `%APPDATA%/TidyTop/logs/tidytop.log` for details.

## Development rule

Do not mark a feature as complete unless it is wired into the app and covered by tests or a written manual verification note. Keep `docs/STATUS.md` and `docs/roadmap/TASKS.md` updated after each completed task.
