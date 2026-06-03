# Changelog

All notable project changes should be recorded here.

## Unreleased

### Changed

- Rewrote the documentation to reflect the real current state of the app.
- Reorganized docs into a dedicated `docs/` tree.
- Repositioned the project as a Windows-first desktop organizer instead of a clone/migration project.
- Removed the unused `TidyTop.Data` project until a real persistence layer is needed.
- Standardized the core domain language around `SmartBox` instead of `Fence`.
- Moved the logo into `assets/logo.png`.
- Replaced placeholder tests with first domain/view-model tests.
- Updated CI to build and test the Windows-first MVP path.

### Started

- Dynamic display of scanned desktop items inside category boxes.

### Not complete yet

- Real desktop icon position control.
- Drag/drop between boxes.
- Box creation/editing UI.
- Persistent named layouts.
- Global quick hide/show hotkey.
- Installer packaging.
