# Sprint Plan

This sprint plan is intentionally sequential. Do not start later sprints until the previous sprint's acceptance criteria are working in the app.

## Sprint 0 — Cleanup and truth reset

Status: completed in this repository cleanup pass.

Goal: remove confusion and make the project maintainable.

Scope:

- Rewrite README.
- Replace old migration/clone docs.
- Add docs index, product doc, status doc, architecture doc, tasks, milestones, workflow, and release checklist.
- Remove empty `TidyTop.Data` project.
- Move brand asset into `assets/`.
- Rename domain direction from Fence to SmartBox.
- Add first useful tests.

Exit criteria:

- Repository structure is clear.
- Docs describe the real app state.
- No empty placeholder tests/classes remain.

## Sprint 1 — Real desktop display

Goal: make the app visibly use real desktop data.

Scope:

- Scan user desktop and common desktop.
- Ignore hidden/system files.
- Group real entries into starter categories.
- Render real names in each SmartBox/category panel.
- Show counts and empty states.
- Add Other/unboxed group.
- Refresh updates the visible data.

Exit criteria:

- A new desktop shortcut appears in TidyTop after Refresh.
- Counts are correct.
- No static fake counts remain.

## Sprint 2 — SmartBox CRUD

Goal: let users create and manage their own boxes.

Scope:

- Add SmartBox dialog.
- Rename SmartBox.
- Delete SmartBox safely.
- Collapse/expand SmartBox.
- Pick a simple color.
- Store SmartBox state in memory.

Exit criteria:

- User can create, rename, collapse, and delete a SmartBox without restarting.

## Sprint 3 — Item movement

Goal: make organization interactive.

Scope:

- Drag an item into a SmartBox.
- Move item between SmartBoxes.
- Move item back to Other/unboxed.
- Prevent duplicates.
- Add a non-drag context menu fallback.

Exit criteria:

- User can manually organize desktop items in the running app.

## Sprint 4 — Save and restore

Goal: make organization persistent.

Scope:

- Create layout JSON repository.
- Save default layout.
- Auto-save changes.
- Load layout on startup.
- Reconcile missing/new desktop files.
- Add save/load tests.

Exit criteria:

- User can organize, close the app, reopen, and see the same layout.

## Sprint 5 — Desktop behavior

Goal: make TidyTop feel like part of the desktop.

Scope:

- Desktop overlay mode.
- SmartBox move/resize on screen.
- Normal window vs overlay toggle.
- Click-through behavior rules.
- Basic manual Windows smoke tests.

Exit criteria:

- User can place SmartBoxes visually over the desktop without the app feeling like a normal dashboard only.

## Sprint 6 — Quick hide/show and tray

Goal: add daily-use convenience.

Scope:

- In-app hide/show.
- Global Windows hotkey.
- Tray icon.
- Startup option.
- Settings UI for hotkey.

Exit criteria:

- User can hide/show TidyTop quickly from keyboard or tray.

## Sprint 7 — v0.1 release candidate

Goal: package the first usable MVP.

Scope:

- Version bump.
- Portable Windows publish.
- Installer decision.
- Release notes.
- Known issues.
- Final smoke test.

Exit criteria:

- A clean Windows user can run TidyTop and complete the MVP flow.
