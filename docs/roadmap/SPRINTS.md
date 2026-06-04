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

## Next Sprint — Settings and polish

Goal: make the new desktop integration understandable and safe for normal users.

Tasks:

1. Add a Settings overlay/window.
2. Let the user toggle:
   - Start hidden.
   - Hide native desktop icons while running.
   - Enable global hotkey.
   - Run on startup placeholder.
3. Add collapse/expand per SmartBox.
4. Add lock/unlock layout.
5. Add first-run onboarding explaining Safe Mode vs Managed Mode.
6. Improve tray icon asset and packaging.

## Release candidate sprint

1. Installer.
2. Uninstall cleanup guidance.
3. Startup registration.
4. Crash/exit safety checks.
5. Multi-monitor smoke tests.
6. Versioned release notes.
