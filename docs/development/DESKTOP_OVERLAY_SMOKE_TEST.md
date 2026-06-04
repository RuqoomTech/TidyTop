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
4. Confirm the compact floating toolbar appears at the top-center.
5. Press **Refresh** and confirm item counts update without crashing.
6. Press **Add box** and confirm a new SmartBox appears on the desktop surface.
7. Press **Auto layout** and confirm boxes reflow into clean columns without overlap.
8. Drag a SmartBox header and confirm the box moves smoothly.
9. Drag the bottom-right resize handle and confirm the box resizes without becoming too small.
10. Double-click an item, or press its **Open** button, and confirm the file/folder/shortcut opens through Windows.
11. Drag an item from one SmartBox and release it over another SmartBox; confirm it moves and counts update.
12. Close the app, run it again, and confirm the moved/resized layout and item assignment load from disk.
13. Open a normal application window and confirm it appears above TidyTop.
14. Minimize all normal windows and confirm TidyTop is visible again on the desktop.

## Known limitations for this pass

- Native Windows desktop icons are not hidden or replaced yet.
- Item drag/drop has no polished visual drop highlight yet.
- There is no tray icon or global hide/show hotkey yet.
- Multi-monitor behavior is not final; the first pass sizes to the primary screen.

## Recovery note

This pass should not hide native desktop icons, so closing TidyTop should not require any recovery action. If a future task adds icon hiding, that task must include automatic restore and documented manual recovery steps.
