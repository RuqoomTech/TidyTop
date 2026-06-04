# Desktop Overlay Smoke Test

Run these manually on Windows after every desktop integration change.

## Build

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project src/TidyTop.App/TidyTop.App.csproj
```

## Overlay behavior

1. TidyTop opens on the desktop without a normal window border.
2. It does not appear as a normal taskbar window.
3. SmartBoxes appear over the wallpaper.
4. Drag a SmartBox header; position should save.
5. Resize a SmartBox; size should save.
6. Restart TidyTop; geometry should restore.

## Item behavior

1. Double-click a desktop item; it should open.
2. Use the compact launch button; the item should open.
3. Drag an item from one SmartBox to another; drop target should highlight.
4. Restart TidyTop; the item should remain in the new SmartBox.
5. Right-click an item and move it through the context menu.

## Desktop integration behavior

1. Check the tray area for the TidyTop icon.
2. Use tray menu → Hide TidyTop; overlay should disappear.
3. Use tray menu → Show TidyTop; overlay should reappear.
4. Press `Ctrl+Alt+T`; overlay should toggle.
5. Use the toolbar or tray menu to hide native desktop icons.
6. Verify desktop files are still present in File Explorer.
7. Use the toolbar or tray menu to show native desktop icons again.
8. If native icons do not immediately reappear, use **Restore icons** in the toolbar or **Restore Windows icons** in the tray.
9. Exit TidyTop from the tray menu; native desktop icons should be visible if TidyTop hid them during the session.
10. Open **Settings** and confirm the Diagnostics section reflects overlay, tray, hotkey, icon, layout, and settings status.
11. Use **Open logs folder** from Settings and confirm `%APPDATA%/TidyTop/logs/tidytop.log` is accessible if a desktop integration step fails.
12. Use **Open app data folder** from Settings and confirm layout/settings files are visible.

## Settings safety

1. Open Settings.
2. Turn on **Start hidden**.
3. Turn off both **Tray icon** and **Global hotkey**.
4. Press **Save settings**. TidyTop should reject this unsafe combination.
5. Press **Reset settings** and confirm safe defaults return.
6. Press **Restore Windows icons** and confirm native icons are visible.

## Persistence safety

1. Open `%APPDATA%/TidyTop`.
2. Confirm `layout.json` and `settings.json` exist after saving.
3. Confirm `.bak` files are created after the second save.
4. Corrupt `layout.json` manually only in a test environment.
5. Start TidyTop and confirm it recovers from `layout.json.bak`.

## Safety rule

TidyTop must never delete, move, rename, or rewrite real Desktop files during this smoke test.
