# Task Backlog

## Done

- [x] Rewrite product docs around the real Windows-first MVP.
- [x] Remove stale prototype claims.
- [x] Replace old Fence vocabulary with SmartBox vocabulary.
- [x] Scan Desktop items.
- [x] Persist layout JSON.
- [x] Render SmartBoxes on a desktop overlay.
- [x] Move and resize SmartBoxes.
- [x] Open desktop files and shortcuts.
- [x] Drag items between SmartBoxes.
- [x] Add right-click item movement.
- [x] Rename SmartBoxes.
- [x] Delete manual SmartBoxes safely.
- [x] Add system tray menu.
- [x] Add show/hide overlay behavior.
- [x] Add `Ctrl+Alt+T` global hotkey.
- [x] Add native desktop icon hide/show.
- [x] Add emergency **Restore Windows icons** action.
- [x] Fix native desktop icon show path to target both DefView and SysListView32.
- [x] Add atomic layout/settings writes with `.bak` recovery.
- [x] Add file logging under `%APPDATA%/TidyTop/logs`.
- [x] Add feature flags for risky desktop integrations.
- [x] Add runtime state model for safer UI/integration coordination.

## Next

- [ ] Build a Settings window/overlay for feature flags.
- [ ] Add a visible diagnostics panel for logs and desktop integration status.
- [ ] Add collapse/expand SmartBox state.
- [ ] Add lock/unlock layout.
- [ ] Add first-run onboarding.
- [ ] Add theme/accent/opacity controls.
- [ ] Add real startup registration.
- [ ] Add better multi-monitor handling.
- [ ] Build installer.

## Safety backlog

- [ ] Add a native desktop icon restore watchdog process or startup recovery task.
- [ ] Add diagnostics when Explorer desktop host is unavailable.
- [ ] Add a safe reset command for settings and layout.
- [ ] Add manual test checklist to release process.
- [ ] Add crash simulation tests around desktop icon visibility.
