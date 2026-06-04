# Milestones

## Milestone 0 — Repository cleanup

Status: done.

Goal: make the repository truthful and organized.

Delivered:

- Accurate docs tree.
- Empty Data project removed.
- Old clone/migration framing removed.
- SmartBox vocabulary standardized.
- Initial tests added.

## Milestone 1 — Foundation rewrite

Status: done in this rewrite pass.

Goal: make the codebase match the MVP scope.

Delivered:

- `DesktopItem`, `SmartBox`, `DesktopLayout`, and `DesktopWorkspace` models.
- Path-based stable item identity.
- Desktop scanner.
- Default rule-based SmartBoxes.
- Catch-all box.
- Layout reconciliation.
- JSON layout persistence.
- JSON settings store.
- View-model driven Avalonia UI.
- Service-level manual SmartBox creation.
- Expanded tests.

Success check:

- Run the app on Windows.
- Real desktop items appear in SmartBoxes.
- Press Refresh and the scan reconciles the layout.
- Press Add SmartBox and a manual box appears.
- Restart should load from `%APPDATA%/TidyTop/layout.json`.

## Milestone 2 — Manual organization

Goal: let users actually organize items.

Deliverables:

- Move item to SmartBox command.
- Move item back to Other / Unboxed.
- Prevent duplicate assignments.
- Context-menu fallback before drag/drop if needed.
- Auto-save after assignment changes.
- Tests for move/reassign/remove behavior.

Success check:

- User can move one desktop item from Other to a manual box and restart with the assignment preserved.

## Milestone 3 — SmartBox CRUD UI

Goal: make boxes manageable.

Deliverables:

- Create SmartBox dialog.
- Rename SmartBox.
- Delete SmartBox safely.
- Collapse/expand SmartBox.
- Simple color/accent selector.

Success check:

- User can create a box named “Work”, rename it, collapse it, delete it, and keep a valid layout.

## Milestone 4 — Visual layout controls

Goal: make SmartBoxes feel like movable desktop objects.

Delivered foundation:

- SmartBoxes render on a canvas.
- Saved X/Y/width/height controls initial placement.

Remaining deliverables:

- Move SmartBox on canvas.
- Resize SmartBox.
- Persist changed X/Y/width/height from UI actions.
- Add grid/snapping later only if needed.

Success check:

- User can place SmartBoxes visually and restart with placement restored.

## Milestone 5 — Desktop integration behavior

Goal: make the app feel like a desktop layer, not a dashboard.

Delivered foundation:

- First desktop overlay mode.
- Borderless transparent window.
- WorkerW/Progman desktop-host attachment attempt.

Remaining deliverables:

- Normal window vs overlay toggle.
- Click-through rules.
- Safe native desktop icon strategy.
- Manual Windows smoke tests.

Success check:

- User can keep boxes over the desktop without breaking normal desktop use.

## Milestone 6 — Quick hide/show and tray

Goal: add daily convenience.

Deliverables:

- In-app hide/show command.
- Global Windows hotkey.
- Tray icon.
- Startup option.

Success check:

- User can instantly hide/show TidyTop from keyboard or tray.

## Milestone 7 — v0.1 release candidate

Goal: package first usable Windows MVP.

Deliverables:

- Portable Windows publish.
- Installer decision.
- Release notes.
- Known issues.
- Smoke test checklist.


## Completed — Desktop overlay foundation

First pass complete: borderless transparent desktop surface, WorkerW/Progman attachment attempt, and canvas-based SmartBox placement. Remaining work: drag/resize boxes, native icon strategy, tray/hotkeys.
