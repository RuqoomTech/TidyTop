# TidyTop Status

TidyTop is now a Windows-first desktop organizer prototype with real desktop integration foundations.

## Working now

- Scans the user and public Desktop folders.
- Groups desktop items into SmartBoxes.
- Opens files, folders, `.lnk`, and `.url` shortcuts through the Windows shell.
- Renders SmartBoxes directly on a transparent desktop overlay.
- Supports moving and resizing SmartBoxes with persisted geometry.
- Supports dragging items between SmartBoxes with visual drop feedback.
- Supports right-click item actions for open and move.
- Supports renaming SmartBoxes and deleting manual SmartBoxes.
- Has a system tray menu:
  - Show / Hide TidyTop
  - Refresh items
  - Auto layout
  - Hide / Show native Windows desktop icons
  - Exit
- Supports `Ctrl+Alt+T` as the first global quick-hide hotkey.
- Supports safe native desktop icon management:
  - Safe mode leaves Windows desktop icons visible.
  - Managed mode hides the native icon view while TidyTop runs.
  - TidyTop restores the captured native desktop icon visibility on exit.
- Persists layout to `%APPDATA%/TidyTop/layout.json`.
- Persists settings to `%APPDATA%/TidyTop/settings.json`.

## Still not complete

- No full settings window yet.
- No startup registration implementation yet.
- No multi-monitor final behavior yet.
- No collapse/expand behavior yet.
- No theme/opacity editor yet.
- No installer yet.
- Native desktop icon management hides the icon view only; it does not virtualize icons or manage Explorer icon positions.

## Current product mode

TidyTop should be treated as a **desktop overlay organizer MVP**, not a finished Fences replacement.

The next release candidate should focus on polish and safety:

1. Settings window.
2. Collapse / expand SmartBoxes.
3. Lock / unlock layout.
4. Multi-monitor handling.
5. Installer and first-run onboarding.
