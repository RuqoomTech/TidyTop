# Current Status

Last updated: 2026-06-03

## Honest summary

TidyTop is currently an early MVP foundation, not a complete desktop organizer.

The app has an Avalonia shell, scans desktop entries, and shows a starter category dashboard. It does not yet fully control desktop icon positions, support drag/drop into user-created boxes, persist named layouts, or provide a global quick hide/show shortcut.

## What works now

| Capability | Real state |
| --- | --- |
| Solution structure | Exists and is cleaned into App/Core/Tests. |
| Avalonia app shell | Exists. |
| Desktop scanning | Started. The app scans desktop files/shortcuts/folders. |
| Category grouping | Started. Items are grouped into starter categories. |
| Category UI | Started. Category boxes render real scanned names. |
| Settings window | Placeholder only. |
| Core models | Started: desktop item, SmartBox, layout, settings, categories. |
| Core services | Started: desktop scan, SmartBox management, settings, layout memory service. |
| Tests | First basic tests added; coverage is still low. |

## What does not work yet

| Capability | State |
| --- | --- |
| Real desktop icon position control | Not implemented. |
| Drag/drop an item into a SmartBox | Not implemented. |
| Create/rename/delete SmartBox from UI | Not implemented. |
| Resize/move SmartBox from UI | Not implemented. |
| Persist layout to disk | Not complete. Settings persistence exists, but full layout persistence still needs implementation. |
| Restore layout on restart | Not implemented. |
| Global hotkeys | Not implemented. |
| Installer | Not implemented. |
| macOS/Linux support | Not supported for real desktop organization behavior. |

## Current MVP risk

The biggest risk is overbuilding UI polish before proving the core loop:

1. Item scan.
2. Box display.
3. User placement.
4. Layout save.
5. Layout restore.

Until that loop works, all advanced features should stay out of scope.

## Definition of done for v0.1

v0.1 is done only when a fresh Windows user can:

1. Install or run the app.
2. See real desktop items.
3. Create at least one SmartBox.
4. Move items into and out of SmartBoxes.
5. Rename, move, and resize a SmartBox.
6. Save the layout automatically.
7. Restart the app and see the same layout.
8. Hide/show TidyTop boxes quickly.
