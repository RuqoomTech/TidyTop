# Desktop Overlay Smoke Test

Run this on Windows after every change to the desktop-hosting code.

## Preconditions

- Windows Explorer is running normally.
- The desktop has at least a few visible files/shortcuts.
- Build succeeds with `dotnet build`.

## Test steps

1. Start the app:

   ```powershell
   dotnet run --project src/TidyTop.App/TidyTop.App.csproj
   ```

2. Confirm TidyTop does not appear as a normal taskbar app window.
3. Confirm SmartBoxes are drawn directly over the desktop/wallpaper area.
4. Confirm the floating toolbar appears at the top-right.
5. Press **Refresh** and confirm item counts update without crashing.
6. Press **Add** and confirm a new SmartBox appears on the desktop surface.
7. Drag a SmartBox header and confirm the box moves smoothly.
8. Drag the bottom-right resize handle and confirm the box resizes without becoming too small.
9. Close the app, run it again, and confirm the moved/resized layout loads from disk.
10. Open a normal application window and confirm it appears above TidyTop.
11. Minimize all normal windows and confirm TidyTop is visible again on the desktop.

## Known limitations for this pass

- Native Windows desktop icons are not hidden or replaced yet.
- There is no tray icon or global hide/show hotkey yet.
- Multi-monitor behavior is not final; the first pass sizes to the primary screen.

## Recovery note

This pass should not hide native desktop icons, so closing TidyTop should not require any recovery action. If a future task adds icon hiding, that task must include automatic restore and documented manual recovery steps.
