# Task Backlog

## Rules

- Keep tasks small enough to verify in one focused session.
- Update this file after each completed task.
- Do not mark UI-only placeholders as complete product features.
- Every core behavior needs a test or a manual verification note.

## Now: Milestone 2 — Manual organization

### M2-01 — Add core item reassignment API

Status: todo.

Acceptance criteria:

- Move item from any SmartBox to another SmartBox.
- Move item to catch-all / Other.
- Item cannot exist in two boxes at once.
- Layout auto-saves after successful move.
- Unit tests cover move, duplicate prevention, and missing item behavior.

### M2-02 — Add UI move command fallback

Status: todo.

Acceptance criteria:

- Each item row exposes a move action.
- User can choose target SmartBox.
- Counts update immediately.
- Works without drag/drop.

### M2-03 — Persist manual assignments after restart

Status: todo.

Acceptance criteria:

- Manual assignment is written to `%APPDATA%/TidyTop/layout.json`.
- App restart restores the assignment.
- Deleted desktop files are removed safely on next scan.

## Next: Milestone 3 — SmartBox CRUD UI

### M3-01 — Create SmartBox dialog

Status: todo.

Acceptance criteria:

- Add SmartBox opens a dialog.
- User can enter title.
- Empty title is rejected.
- Box appears immediately and saves.

### M3-02 — Rename SmartBox

Status: todo.

Acceptance criteria:

- User can rename a manual SmartBox.
- System boxes can be renamed only if we intentionally allow it.
- Rename persists after restart.

### M3-03 — Delete SmartBox safely

Status: todo.

Acceptance criteria:

- User confirms deletion.
- Items inside deleted SmartBox move to Other.
- System catch-all cannot be deleted.

### M3-04 — Collapse/expand SmartBox

Status: todo.

Acceptance criteria:

- Collapse shows title and count only.
- State persists in layout JSON.

## Next: Milestone 4 — Drag/drop

### M4-01 — Drag item between boxes

Status: todo.

Acceptance criteria:

- Item can move between boxes with drag/drop.
- Drop targets are visually clear.
- Counts update immediately.

### M4-02 — Drag item back to Other

Status: todo.

Acceptance criteria:

- Item can be returned to catch-all.
- Assignment is removed from previous box.

## Next: Milestone 5 — Visual placement

### M5-01 — Switch SmartBox surface from WrapPanel to Canvas

Status: todo.

Acceptance criteria:

- SmartBox X/Y from layout model controls placement.
- Window still supports scrolling or a large workspace safely.

### M5-02 — Move and resize SmartBox

Status: todo.

Acceptance criteria:

- User can drag a SmartBox header to move.
- User can resize a SmartBox.
- Placement persists after restart.

## Later

- Real icon extraction.
- Overlay mode.
- Global hotkey.
- Tray icon.
- Installer.
- Release checklist.

## Done

### D-01 — Remove misleading docs

Completed in cleanup pass.

### D-02 — Move docs into structured docs tree

Completed in cleanup pass.

### D-03 — Remove empty Data project

Completed in cleanup pass.

### D-04 — Replace placeholder tests

Completed with initial model/view-model tests.

### D-05 — Rewrite foundation around scoped MVP

Completed in foundation rewrite.

Notes:

- Replaced duplicated icon-in-box model with path-based SmartBox assignments.
- Added scanner, reconciler, JSON layout store, JSON settings store, workspace service.
- Replaced code-behind desktop grouping with view-model-bound UI.
- Added tests for matching, reconciliation, persistence, and main VM behavior.
