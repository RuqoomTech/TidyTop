# Architecture

TidyTop is split into a small core library and an Avalonia Windows desktop app.

## `TidyTop.Core`

Core owns desktop-independent business logic.

Main concepts:

- `DesktopItem` — a scanned file, folder, shortcut, or URL from Desktop folders.
- `SmartBox` — a persisted desktop grouping area.
- `SmartBoxRule` — rule-based grouping by extension/type.
- `DesktopLayout` — persisted SmartBox geometry and assignments.
- `DesktopWorkspace` — reconciled runtime view of layout + current scanned items.
- `AppSettings` — independent app settings such as native icon mode and hotkey preference.

Core services:

- `DesktopScanner`
- `JsonLayoutStore`
- `JsonAppSettingsStore`
- `LayoutReconciler`
- `DesktopWorkspaceService`
- `DesktopItemLauncher`

## `TidyTop.App`

The app owns Windows/Avalonia integration.

Main pieces:

- `MainWindow` — transparent desktop overlay window and interaction surface.
- `MainWindowViewModel` — workspace state, settings state, commands, and UI status.
- `WindowsDesktopOverlayHost` — attaches TidyTop to the desktop WorkerW/Progman host.
- `WindowsNativeDesktopIconService` — hides/shows the native Windows desktop icon view without modifying files.
- `WindowsGlobalHotkeyService` — low-level Windows keyboard hook for `Ctrl+Alt+T`.
- Avalonia `TrayIcon` — tray menu for show/hide, refresh, auto layout, native icon toggle, and exit.

## Safety design

TidyTop currently does **not** move, delete, or rename real Desktop files.

Manual organization stores only normalized file paths inside `layout.json`.

Managed desktop mode hides the Explorer desktop icon view only. It does not change icon positions or file locations.

On exit, TidyTop restores the native desktop icon visibility it captured at startup.
