# TidyTop

TidyTop is a Windows-first desktop organization app built with .NET 8 and Avalonia. The goal is simple: help users group desktop shortcuts and files into clean, visual boxes, then save and restore that layout reliably.

The current repository is an early MVP foundation. It can scan desktop items and display them in starter category boxes, but full desktop icon control, drag/drop placement, persistent layouts, resizing, and hotkeys are still planned work.

## Product direction

TidyTop should be positioned as a lightweight desktop organizer, not as a clone of another product. The app should focus on the smallest useful workflow first:

1. Scan the desktop.
2. Display real desktop items inside visual boxes.
3. Let the user create, rename, move, and resize boxes.
4. Let the user move items between boxes.
5. Save the layout.
6. Restore the same layout after restart.
7. Add a quick hide/show shortcut.

## Current status

| Area | Status |
| --- | --- |
| Avalonia shell | Started |
| Desktop scan | Started |
| Category display | Started |
| Real icon rendering | Planned |
| User-created boxes | Planned |
| Drag/drop between boxes | Planned |
| Layout persistence | Planned |
| Quick hide/show hotkey | Planned |
| Installer/release packaging | Planned |

See [`docs/STATUS.md`](docs/STATUS.md) for the detailed state.

## Tech stack

- .NET 8
- Avalonia UI
- C#
- xUnit
- Windows-first desktop integration

## Repository structure

```text
TidyTop/
├── assets/                    # Brand assets and static images
├── docs/                      # Product, architecture, roadmap, and task docs
├── src/
│   ├── TidyTop.App/           # Avalonia UI application
│   └── TidyTop.Core/          # Domain models and core services
├── tests/
│   ├── TidyTop.App.Tests/     # App/view-model level tests
│   └── TidyTop.Core.Tests/    # Domain and service tests
├── Directory.Build.props      # Shared .NET build settings
├── TidyTop.sln                # Visual Studio solution
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

## Test

```bash
dotnet test
```

## Development rule

Do not mark a feature as complete until it works in the app and has at least one useful test or manual verification note. Keep `docs/STATUS.md` and `docs/roadmap/TASKS.md` updated after each completed task.
