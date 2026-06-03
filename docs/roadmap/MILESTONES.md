# Milestones

## Milestone 0 — Repository cleanup

Status: mostly done in this pass.

Goal: make the repository truthful and organized.

Deliverables:

- Accurate README.
- Ordered docs tree.
- Clear MVP definition.
- Old overclaiming docs removed.
- Empty Data project removed.
- Domain naming moved toward SmartBox.
- Placeholder tests replaced with useful first tests.

## Milestone 1 — Real display MVP

Goal: show real scanned desktop items inside category boxes.

Deliverables:

- Scan user desktop and common desktop.
- Include shortcuts, URLs, executable files, folders, and common document files.
- Render item names in the UI.
- Show real counts per box.
- Add an unboxed/other group.
- Refresh button updates the UI.

Success check:

- Add a shortcut to the desktop.
- Click Refresh.
- The item appears in the right box or in Other.

## Milestone 2 — SmartBox CRUD

Goal: let users manage boxes manually.

Deliverables:

- Create SmartBox.
- Rename SmartBox.
- Delete SmartBox.
- Change SmartBox color.
- Collapse/expand SmartBox.
- Store SmartBox metadata in memory first.

Success check:

- User can create a box named “Work”, rename it, collapse it, and delete it.

## Milestone 3 — Drag/drop organization

Goal: make organization interactive.

Deliverables:

- Drag desktop item row/card into a SmartBox.
- Move item between SmartBoxes.
- Move item back to Other.
- Prevent duplicates.
- Update counts immediately.

Success check:

- User can move one desktop item from Other to Work and see the count update.

## Milestone 4 — Layout persistence

Goal: make organization survive restart.

Deliverables:

- Save default layout to JSON.
- Auto-save after box changes or item moves.
- Load layout on app startup.
- Handle missing/deleted desktop files safely.
- Add tests for save/load.

Success check:

- User organizes items, closes app, reopens app, and sees the same organization.

## Milestone 5 — Desktop overlay behavior

Goal: make the app feel like a desktop layer, not just a dashboard.

Deliverables:

- Borderless desktop-positioned window mode.
- Toggle normal window vs overlay mode.
- Click-through rules defined and tested manually.
- Keep boxes above wallpaper without blocking normal desktop use unnecessarily.

Success check:

- User can organize boxes visually over the desktop and still use normal desktop interactions.

## Milestone 6 — Quick hide/show

Goal: give users instant visual cleanup.

Deliverables:

- In-app hide/show command.
- Global Windows hotkey.
- Tray icon with show/hide/exit.
- Setting for hotkey.

Success check:

- Press configured hotkey and all TidyTop boxes hide/show quickly.

## Milestone 7 — v0.1 release candidate

Goal: package the first usable Windows MVP.

Deliverables:

- Windows publish profile.
- Installer or zipped portable build.
- Smoke test checklist completed.
- Known issues documented.
- Versioned release notes.

Success check:

- A clean Windows machine can run the app and complete the v0.1 user flow.
