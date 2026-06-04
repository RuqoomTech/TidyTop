# TidyTop Status

TidyTop is a Windows-first desktop organizer prototype. The current focus is stability and safe desktop integration, not adding more visual features.

## Working now

- Scans the user and public Desktop folders.
- Groups desktop items into SmartBoxes.
- Opens files, folders, `.lnk`, and `.url` shortcuts through the Windows shell.
- Renders SmartBoxes on a transparent desktop overlay.
- Supports moving and resizing SmartBoxes with persisted geometry.
- Supports dragging items between SmartBoxes with visual drop feedback.
- Supports right-click item actions for open and move.
- Supports renaming SmartBoxes and deleting manual SmartBoxes.
- Has a system tray menu for show/hide, refresh, auto layout, icon control, restore icons, and safe exit.
- Supports `Ctrl+Alt+T` as the first global quick-hide hotkey.
- Supports native desktop icon visibility control without touching real files.
- Adds an always-available **Restore icons** command for the Windows desktop icon bug/safety case.
- Persists layout to `%APPDATA%/TidyTop/layout.json`.
- Persists settings to `%APPDATA%/TidyTop/settings.json`.
- Writes logs to `%APPDATA%/TidyTop/logs/tidytop.log`.
- Saves layout/settings atomically with `.tmp` writes and `.bak` recovery.

## Hardening added

- Central runtime state model for overlay visibility, native icon visibility, dragging, editor state, shutdown state, and last error.
- Feature flags in settings:
  - `EnableDesktopOverlayHost`
  - `EnableNativeDesktopIconControl`
  - `EnableTrayIcon`
  - `EnableGlobalHotkey`
  - `EnableDragDrop`
- Desktop icon show/hide now targets both Explorer's `SHELLDLL_DefView` and the child `SysListView32` icon list.
- Exit path prefers restoring Windows desktop icons to visible if TidyTop hid them during the session.
- Broken layout/settings JSON can recover from `.bak` files.

## Still not complete

- No full settings window yet.
- No startup registration implementation yet.
- No multi-monitor final behavior yet.
- No collapse/expand behavior yet.
- No theme/opacity editor yet.
- No installer yet.
- Native desktop icon management hides/shows Explorer's icon view only; it does not virtualize icons or manage Explorer icon positions.

## Current product mode

TidyTop should be treated as a **desktop overlay organizer MVP**, not a finished Fences replacement.

The next release candidate should focus on:

1. Settings window for feature flags and safety controls.
2. Collapse / expand SmartBoxes.
3. Lock / unlock layout.
4. Multi-monitor handling.
5. Installer and first-run onboarding.
