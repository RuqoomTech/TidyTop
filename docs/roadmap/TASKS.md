# Task Backlog

## Rules

- Keep tasks small enough to complete and verify in one focused session.
- Update this file after finishing a task.
- Move completed items to `Done` with a short note.
- Do not mark UI-only placeholders as complete product features.

## Now: Milestone 1 — Real display MVP

### M1-01 — Render scanned desktop items in category boxes

Status: started.

Acceptance criteria:

- Desktop scan result appears in UI, not just status text.
- Counts update per category.
- Empty boxes show an empty-state message.
- Other/unboxed items are visible.

### M1-02 — Include common desktop folder

Status: started.

Acceptance criteria:

- User desktop and common/public desktop are scanned.
- Duplicate paths are ignored.
- Hidden/system files are ignored.

### M1-03 — Replace code-behind categorization with shared core categorizer

Status: todo.

Acceptance criteria:

- Category rules live in Core.
- UI only renders the grouped result.
- Unit tests cover at least five categorization cases.

### M1-04 — Render icons/images when available

Status: todo.

Acceptance criteria:

- Shortcut/file icon extraction is displayed when available.
- Fallback icon is shown when extraction fails.
- App does not crash on non-Windows runtime.

## Next: Milestone 2 — SmartBox CRUD

### M2-01 — Create SmartBox dialog

Status: todo.

Acceptance criteria:

- Add Box opens a real dialog.
- User can enter title and choose starter color.
- Created SmartBox appears immediately.

### M2-02 — Rename SmartBox

Status: todo.

Acceptance criteria:

- User can rename a SmartBox.
- Empty names are rejected.
- UI updates immediately.

### M2-03 — Delete SmartBox safely

Status: todo.

Acceptance criteria:

- User confirms deletion.
- Items inside deleted SmartBox move to Other/unboxed.
- No duplicate item entries remain.

### M2-04 — Collapse/expand SmartBox

Status: todo.

Acceptance criteria:

- User can collapse and expand a SmartBox.
- Collapsed SmartBox shows title and count only.
- State is included in layout model.

## Next: Milestone 3 — Drag/drop

### M3-01 — Drag item between visual boxes

Status: todo.

Acceptance criteria:

- Item can move between boxes.
- Counts update.
- Item cannot exist in two boxes at once.

### M3-02 — Keyboard move command

Status: todo.

Acceptance criteria:

- Selected item can be moved using a context menu or keyboard command.
- This supports users who do not like drag/drop.

## Next: Milestone 4 — Layout persistence

### M4-01 — Layout JSON repository

Status: todo.

Acceptance criteria:

- Save layout to `%APPDATA%/TidyTop/layouts/default.json`.
- Load layout safely.
- Corrupt JSON is backed up and replaced with default layout.

### M4-02 — Auto-save layout changes

Status: todo.

Acceptance criteria:

- Changes are saved after a short debounce.
- App close flushes pending changes.
- Tests cover save/load round trip.

### M4-03 — Missing item reconciliation

Status: todo.

Acceptance criteria:

- If a file no longer exists, it is removed or marked missing safely.
- New desktop items appear in Other or auto-category.

## Later: Milestones 5-7

- Overlay mode.
- SmartBox positioning and resizing.
- Global hotkeys.
- Tray icon.
- Installer.
- Release checklist.

## Done

### D-01 — Remove misleading docs

Completed in repository cleanup.

### D-02 — Move docs into structured docs tree

Completed in repository cleanup.

### D-03 — Remove empty Data project

Completed in repository cleanup.

### D-04 — Replace placeholder tests

Completed with initial model/view-model tests. More coverage still needed.
