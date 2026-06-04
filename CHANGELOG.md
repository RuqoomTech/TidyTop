# Changelog

All notable project changes should be recorded here.

## Unreleased

### Added

- SmartBox desktop interactions: drag the header to move a box and drag the bottom-right handle to resize it.
- Geometry autosave via `UpdateSmartBoxGeometryAsync`, writing SmartBox placement back to layout JSON on pointer release.
- Core SmartBox geometry clamping with minimum width/height.
- Desktop overlay foundation: borderless transparent main window hidden from taskbar.
- Windows `WorkerW`/`Progman` host attachment service behind `IDesktopOverlayHost`.
- Canvas-based SmartBox rendering using saved X/Y/width/height layout values.


### Changed

- Rewrote the codebase foundation around the real Windows-first MVP scope.
- Replaced the old mixed `DesktopIcon`/category/service structure with focused domain models:
  - `DesktopItem`
  - `SmartBox`
  - `SmartBoxRule`
  - `DesktopLayout`
  - `DesktopWorkspace`
- Changed SmartBox persistence to store normalized desktop item paths instead of duplicated item objects.
- Replaced code-behind grouping/rendering with MVVM-bound Avalonia UI.
- Simplified application startup to Microsoft DI + Avalonia.
- Removed unused converters and placeholder settings window.
- Changed `TidyTop.Core` to target `net8.0` and keep platform-heavy behavior outside the core domain.
- Updated docs to match the new foundation and next milestones.

### Added

- Desktop scanner for user and public desktop folders.
- Default SmartBox factory for first-run system boxes.
- Catch-all `Other / Unboxed` SmartBox.
- Layout reconciler for new, deleted, duplicate, and unassigned desktop items.
- JSON layout persistence at `%APPDATA%/TidyTop/layout.json`.
- JSON settings persistence at `%APPDATA%/TidyTop/settings.json`.
- High-level `DesktopWorkspaceService` for scan → reconcile → save.
- UI commands for refresh, add SmartBox, save, and reset layout.
- Tests for SmartBox rules, SmartBox assignment, layout cloning, reconciliation, JSON layout persistence, and main view-model loading.

### Not complete yet

- Manual item movement between boxes.
- Rename/delete SmartBox UI.
- Drag/drop.
- Full Fences-like native desktop icon hiding/replacement behavior.
- Global quick hide/show hotkey.
- Tray icon.
- Installer packaging.
