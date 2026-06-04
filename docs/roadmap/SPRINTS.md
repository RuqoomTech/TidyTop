# Sprint Plan

## Sprint 0 — Cleanup and truth reset

Status: done.

Scope:

- Rewrite docs.
- Remove empty Data project.
- Remove misleading completed claims.
- Align language around SmartBox.

## Sprint 1 — Foundation rewrite

Status: done in this pass.

Scope:

- Replace old mixed models with clean domain models.
- Move categorization/reconciliation into Core.
- Add layout JSON persistence.
- Add settings JSON store.
- Replace code-behind rendering with view-model binding.
- Add tests for the foundation.

Exit criteria:

- App loads real desktop items into SmartBoxes.
- Layout is saved to JSON.
- New/deleted items reconcile safely.
- Tests cover the core behavior.

## Sprint 2 — Item movement

Status: first pass done.

Goal: make manual organization real.

Completed scope:

- Added core methods for moving items between boxes.
- Added shell launcher so double-click opens files, folders, shortcuts, and URLs.
- Added drag item to another SmartBox.
- Auto-save after item move.
- Added tests for app-level launch/move behavior.

Remaining polish:

- Add right-click move fallback.
- Add visual drop-target highlight.
- Add explicit move-to-Other command.

Exit criteria:

- User can manually assign an item to a box and see it survive restart.

## Sprint 3 — SmartBox CRUD UI

Goal: make manual boxes useful.

Scope:

- Create SmartBox dialog.
- Rename SmartBox.
- Delete SmartBox.
- Collapse/expand SmartBox.
- Simple accent color picker.

Exit criteria:

- User can manage boxes without editing JSON.

## Sprint 4 — Drag/drop polish

Goal: make organization feel natural.

Scope:

- Improve visual drag ghost/drop target feedback.
- Drop item into Other explicitly.
- Add duplicate-prevention tests at the service level.
- Add keyboard/context-menu fallback.

Exit criteria:

- User can organize items with drag/drop and clear visual feedback.

## Sprint 5 — Visual placement

Status: done in desktop interaction pass.

Goal: make boxes spatial.

Completed scope:

- SmartBoxes render on a canvas.
- Saved placement values drive first render.
- User can drag a SmartBox header to move it.
- User can drag the bottom-right handle to resize it.
- Placement changes are persisted to layout JSON on pointer release.

Exit criteria:

- User can position SmartBoxes and restart with placement restored.

## Sprint 6 — Desktop integration and quick hide

Goal: make it usable as a daily desktop tool.

Already done:

- First desktop overlay surface.

Scope remaining:

- Normal window vs overlay toggle.
- Safe native desktop icon strategy.
- Global hotkey.
- Tray icon.
- Startup option.

Exit criteria:

- User can hide/show TidyTop quickly and keep it integrated with the desktop.

## Sprint 7 — v0.1 RC

Goal: package and verify.

Scope:

- Publish portable Windows build.
- Installer decision.
- Release notes.
- Manual smoke tests on clean Windows.
