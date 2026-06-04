# Task Backlog

## Rules

- Keep tasks small enough to verify in one focused session.
- Update this file after each completed task.
- Do not mark UI-only placeholders as complete product features.
- Every core behavior needs a test or a manual verification note.

## Now: Milestone 2 — Manual organization

### M2-01 — Add core item reassignment API

Status: done.

Acceptance criteria:

- Move item from any SmartBox to another SmartBox.
- Move item to catch-all / Other.
- Item cannot exist in two boxes at once.
- Layout auto-saves after successful move.
- Unit tests cover move, duplicate prevention, and missing item behavior.

### M2-02 — Add UI move command fallback

Status: partially done.

Current state:

- Item can be dragged from one SmartBox and dropped onto another.
- Right-click/context-menu fallback is still todo.

Acceptance criteria:

- Each item row exposes a move action.
- User can choose target SmartBox.
- Counts update immediately.
- Works without drag/drop.

### M2-03 — Persist manual assignments after restart

Status: done for drag-to-box movement.

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

## Next: Milestone 4 — Drag/drop polish

### M4-01 — Drag item between boxes

Status: first pass done.

Acceptance criteria:

- Item can move between boxes with drag/drop.
- Drop targets are visually clear.
- Counts update immediately.

### M4-02 — Drag item back to Other

Status: first pass works when dropping onto the Other / Unboxed box; still needs stronger visual feedback.

Acceptance criteria:

- Item can be returned to catch-all.
- Assignment is removed from previous box.

## Next: Milestone 6 — Native desktop integration

### M6-01 — Safe native icon handling strategy

Status: todo.

Acceptance criteria:

- Decide whether v0.1 hides native Windows desktop icons while TidyTop is running, leaves them visible, or uses another approach.
- If hiding icons is chosen, app must restore them on normal exit and document recovery steps for crash cases.
- No destructive file moves.

### M6-02 — Real icon extraction

Status: todo.

Acceptance criteria:

- File/folder/shortcut icons are shown instead of emoji fallback icons.
- Missing icon extraction falls back safely.

## Later

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

### D-06 — First desktop overlay pass

Completed in desktop-overlay pass.

Notes:

- Main window is now borderless, transparent, and hidden from the taskbar.
- Added `IDesktopOverlayHost` and `WindowsDesktopOverlayHost`.
- Attempts to attach TidyTop to Windows WorkerW/Progman desktop host.
- Switched SmartBox rendering from WrapPanel to Canvas using saved coordinates.
- Native Windows desktop icon hiding/replacement is intentionally not done yet.

### D-07 — Visual SmartBox move and resize

Completed in desktop interaction pass.

Notes:

- SmartBox header dragging updates `X` and `Y` in the view model.
- Bottom-right resize handle updates `Width` and `Height`.
- Geometry is clamped to safe minimum values.
- Layout is auto-saved through `IDesktopWorkspaceService.UpdateSmartBoxGeometryAsync(...)` on pointer release.
- Added tests for core SmartBox geometry clamping.

### D-08 — Open desktop items and basic manual item movement

Completed in item organization pass.

Notes:

- Added `IDesktopItemLauncher` and shell-based launching.
- Double-clicking an item or pressing Open launches files, folders, `.lnk` shortcuts, and `.url` shortcuts through Windows shell behavior.
- Added workspace methods for moving an item into a target SmartBox.
- Added basic drag-to-box item reassignment in the desktop overlay.
- Layout auto-saves after item movement.
- Improved the overlay layout with clearer item cards, instructions, and empty-state guidance.
