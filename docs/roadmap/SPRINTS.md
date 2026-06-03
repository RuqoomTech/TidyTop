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

Goal: make manual organization real.

Scope:

- Add core methods for moving items between boxes.
- Add command on item row: Move to box.
- Add command on item row: Move to Other.
- Auto-save after item move.
- Add tests for reassignment.

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

## Sprint 4 — Drag/drop

Goal: make organization feel natural.

Scope:

- Drag item between boxes.
- Drop item into Other.
- Visual drop targets.
- Duplicate prevention.

Exit criteria:

- User can organize items with drag/drop and counts update immediately.

## Sprint 5 — Visual placement

Goal: make boxes spatial.

Scope:

- Switch from wrap dashboard to canvas layout.
- Move SmartBox.
- Resize SmartBox.
- Persist placement.

Exit criteria:

- User can position SmartBoxes and restart with placement restored.

## Sprint 6 — Desktop overlay and quick hide

Goal: make it usable as a daily desktop tool.

Scope:

- Overlay mode.
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
