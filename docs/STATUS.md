# Current Status

Last updated: 2026-06-03

## Honest summary

TidyTop is now a cleaner MVP foundation. The app is still not a full desktop overlay/organizer, but the core architecture now matches the intended scope: scan real desktop items, reconcile them into SmartBoxes, persist layout JSON, and expose the result through view models.

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
| Manual SmartBox creation | Service and button create a basic manual SmartBox. No dialog yet. |
| Tests | Meaningful first tests exist for rules, layout, persistence, reconciliation, and app VM. |

## What does not work yet

| Capability | State |
| --- | --- |
| Rename/delete SmartBox from UI | Not implemented. |
| Drag/drop an item into a SmartBox | Not implemented. |
| Move item between SmartBoxes | Not implemented. |
| Resize/move SmartBox visually | Not implemented. |
| Real desktop overlay/window positioning | Not implemented. |
| Real file/shortcut icon extraction | Not implemented in this rewrite; emoji/fallback icons are used. |
| Global hotkeys | Not implemented. |
| Tray icon | Not implemented. |
| Installer | Not implemented. |
| macOS/Linux support | Not supported for product behavior. |

## Current MVP risk

The biggest risk is jumping into overlay/hotkey polish before finishing manual organization. The next proof should be:

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
