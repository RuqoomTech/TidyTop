# Current Status

Last updated: 2026-06-04

## Honest summary

TidyTop is now a cleaner MVP foundation with a first Windows desktop-overlay pass. The app scans real desktop items, reconciles them into SmartBoxes, persists layout JSON, and renders SmartBoxes on a transparent canvas attached to the desktop host when possible.

## What works now

| Capability | Real state |
| --- | --- |
| Solution structure | Clean App/Core/Tests split. |
| Avalonia shell | Exists and binds to view models. |
| Desktop scanning | Scans user desktop and public desktop folders. |
| Hidden/system filtering | Implemented in scanner. |
| Stable item identity | Implemented using normalized path + stable hash ID. |
| Default SmartBoxes | Implemented in Core through rule-based boxes. |
| Other / Unboxed | Implemented as catch-all SmartBox. |
| Layout reconciliation | Implemented for new, deleted, duplicated, and unassigned items. |
| Layout persistence | Implemented as `%APPDATA%/TidyTop/layout.json`. |
| Settings persistence | Implemented as `%APPDATA%/TidyTop/settings.json`. |
| UI rendering | Bound to `MainWindowViewModel` and `SmartBoxViewModel`. |
| Desktop overlay shell | Implemented first pass: borderless, transparent, hidden from taskbar, and attached to WorkerW/Progman when possible. |
| Canvas placement | Implemented first pass using `SmartBox.X`, `Y`, `Width`, and `Height`. |
| Manual SmartBox creation | Service and button create a basic manual SmartBox. No dialog yet. |
| Tests | Meaningful first tests exist for rules, layout, persistence, reconciliation, and app VM. |

## What does not work yet

| Capability | State |
| --- | --- |
| Rename/delete SmartBox from UI | Not implemented. |
| Drag/drop an item into a SmartBox | Not implemented. |
| Move item between SmartBoxes | Not implemented. |
| Resize/move SmartBox visually | Not implemented; boxes render from saved coordinates but cannot be dragged/resized yet. |
| Native desktop icon hiding/replacement | Not implemented; Windows desktop icons may still exist behind/above the TidyTop surface. |
| Real file/shortcut icon extraction | Not implemented; emoji/fallback icons are used. |
| Global hotkeys | Not implemented. |
| Tray icon | Not implemented. |
| Installer | Not implemented. |
| macOS/Linux support | Not supported for product behavior. |

## Current MVP risk

The biggest risk is assuming the overlay is already a complete Fences-like replacement. The next proof should be:

1. create or rename a SmartBox,
2. move an item into it,
3. save automatically,
4. restart and see the same assignment.

## Definition of done for v0.1

v0.1 is done only when a fresh Windows user can:

1. run the app,
2. see real desktop items,
3. create at least one SmartBox,
4. move items into and out of SmartBoxes,
5. rename, move, and resize a SmartBox,
6. close and reopen the app with the same layout restored,
7. hide/show TidyTop boxes quickly.
