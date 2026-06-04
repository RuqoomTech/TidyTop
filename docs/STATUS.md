# Current Status

Last updated: 2026-06-04

## Honest summary

TidyTop is now a cleaner MVP foundation with a first Windows desktop-overlay pass. The app scans real desktop items, reconciles them into SmartBoxes, persists layout JSON, renders SmartBoxes on a transparent canvas attached to the desktop host when possible, supports moving/resizing SmartBoxes directly on the desktop, opens desktop items through the OS shell, supports basic drag-to-box item reassignment, and now has a cleaner desktop UI with an auto-layout command for tidying box placement.

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
| UI rendering | Bound to `MainWindowViewModel` and `SmartBoxViewModel`; current pass uses cleaner glass SmartBoxes, compact item rows, centered toolbar, and reduced visual noise. |
| Desktop overlay shell | Implemented first pass: borderless, transparent, hidden from taskbar, and attached to WorkerW/Progman when possible. |
| Canvas placement | Implemented using `SmartBox.X`, `Y`, `Width`, and `Height`. |
| Visual SmartBox movement | Implemented: drag a SmartBox header to move it. |
| Visual SmartBox resizing | Implemented: drag the bottom-right handle to resize it. |
| Geometry autosave | Implemented: move/resize commits to `%APPDATA%/TidyTop/layout.json` on pointer release. |
| Item launching | Implemented: double-click an item or press Open to launch file/folder/shortcut/URL through the OS shell. |
| Manual item movement | Implemented first pass: drag an item from one SmartBox and drop it onto another. Assignment auto-saves. |
| Manual SmartBox creation | Service and button create a basic manual SmartBox. No dialog yet. |
| Auto layout | Implemented: command reflows SmartBoxes into balanced desktop columns and saves the result. |
| Tests | Meaningful first tests exist for rules, layout, persistence, reconciliation, item launching, movement, and app VM. |

## What does not work yet

| Capability | State |
| --- | --- |
| Rename/delete SmartBox from UI | Not implemented. |
| Drag/drop an item into a SmartBox | Implemented first pass; still needs visual drop highlight and context-menu fallback. |
| Move item between SmartBoxes | Implemented first pass with drag-to-box and autosave. |
| Native desktop icon hiding/replacement | Not implemented; Windows desktop icons may still exist behind/above the TidyTop surface. |
| Real file/shortcut icon extraction | Not implemented; emoji/fallback icons are used. |
| Global hotkeys | Not implemented. |
| Tray icon | Not implemented. |
| Installer | Not implemented. |
| macOS/Linux support | Not supported for product behavior. |

## Current MVP risk

The biggest risk is assuming the overlay is already a complete Fences-like replacement. The next proof should be:

1. add drop-target highlight/drag ghost,
2. add a right-click move fallback,
3. create/rename/delete SmartBoxes from UI,
4. decide safe native Windows desktop icon handling.

## Definition of done for v0.1

v0.1 is done only when a fresh Windows user can:

1. run the app,
2. see real desktop items,
3. create at least one SmartBox,
4. move items into and out of SmartBoxes,
5. rename a SmartBox and move/resize it on the desktop,
6. close and reopen the app with the same layout restored,
7. hide/show TidyTop boxes quickly.
