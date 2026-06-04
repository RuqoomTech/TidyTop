# Sprint Plan

## Completed Foundation Sprints

### Sprint 1 — Core workspace foundation

- Desktop item model.
- SmartBox model.
- Rule-based grouping.
- Layout persistence.
- Settings persistence foundation.

### Sprint 2 — Desktop overlay foundation

- Borderless transparent overlay.
- Windows desktop host attachment.
- Canvas-based SmartBox positioning.

### Sprint 3 — SmartBox interactions

- Move SmartBoxes.
- Resize SmartBoxes.
- Persist geometry.
- Auto layout.

### Sprint 4 — Item organization

- Open desktop items.
- Drag items between SmartBoxes.
- Right-click move menu.
- SmartBox rename/delete editor.

### Sprint 5 — Desktop integration

- System tray menu.
- Show/hide overlay.
- `Ctrl+Alt+T` global quick-hide hotkey.
- Safe native desktop icon hide/show.
- Restore captured native icon visibility on exit.
- Persist desktop integration settings.

### Sprint 6 — Settings and diagnostics

- Settings & Diagnostics panel.
- Feature flag toggles for overlay host, tray, hotkey, drag/drop, native icon control, and auto-organize.
- Emergency actions for restoring Windows icons, resetting layout, resetting settings, and resetting all local TidyTop data.
- Live diagnostics for overlay visibility, desktop host attachment, native icons, tray, hotkey, layout, settings, and last error.
- Folder shortcuts for logs and app data.
- Safety guard that prevents Start hidden when both tray and hotkey are disabled.

## Next Sprint — SmartBox usability polish

Goal: make day-to-day SmartBox control smoother now that the safety surface exists.

Tasks:

1. Add collapse/expand per SmartBox.
2. Add lock/unlock layout.
3. Add first-run onboarding explaining Safe Mode vs Managed Mode.
4. Improve tray icon asset and packaging.
5. Add theme/accent/opacity controls.

## Release candidate sprint

1. Installer.
2. Uninstall cleanup guidance.
3. Startup registration.
4. Crash/exit safety checks.
5. Multi-monitor smoke tests.
6. Versioned release notes.
