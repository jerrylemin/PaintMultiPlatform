# Codex Test Report

## Commands Run

```powershell
Get-Location; Get-ChildItem -Force
Get-ChildItem -Recurse -File
Get-Content -Raw README.md
Get-Content -Raw docs\codex_architecture_plan.md
Get-Content -Raw docs\codex_progress.md
Get-Content -Raw docs\codex_project_audit.md
Get-Content -Raw docs\codex_test_report.md
Get-Content -Raw BasicDrawingApp\BasicDrawingApp.csproj
Get-Content -Raw BasicDrawingApp\MauiProgram.cs
Get-Content -Raw BasicDrawingApp\Platforms\Android\AndroidManifest.xml
Get-Content -Raw BasicDrawingApp\Views\MainPage.xaml
Get-Content -Raw BasicDrawingApp\Views\MainPage.xaml.cs
Get-Content -Raw BasicDrawingApp\ViewModels\MainViewModel.cs
Get-Content -Raw BasicDrawingApp\Services\DrawingBinarySerializer.cs
Get-Content -Raw BasicDrawingApp\Services\ImageExportService.cs
Get-Content -Raw BasicDrawingApp\Services\FilePickerService.cs
Get-Content -Raw BasicDrawingApp\Controls\DrawingCanvasView.cs
Get-Content -Raw BasicDrawingApp\Controls\DrawingRenderer.cs
dotnet build BasicDrawingApp\BasicDrawingApp.csproj
dotnet build BasicDrawingApp\BasicDrawingApp.csproj -f net9.0-android
```

## Build Results

- `dotnet build BasicDrawingApp\BasicDrawingApp.csproj`: PASS.
- `dotnet build BasicDrawingApp\BasicDrawingApp.csproj -f net9.0-android`: PASS.
- Both builds finished with `0 Warning(s), 0 Error(s)`.
- The .NET 10 preview SDK prints `NETSDK1057`; this is an SDK support-policy message, not a project warning.

## Windows Expected Test Result

- `Save .bdraw` opens Windows Save Picker and writes a non-empty `.bdraw` file to the selected path.
- `Load .bdraw` opens FilePicker and reloads the selected file.
- `Export PNG` opens Save Picker and writes a PNG with white background.
- `Export JPEG` opens Save Picker and writes a JPEG with white background.
- Exported image files should open in Photos/Paint because the encoder writes real PNG/JPEG bytes through SkiaSharp.

Manual Windows UI execution was not run from this terminal session.

## Android Expected Test Result

- `Save .bdraw to App Files` writes to `FileSystem.AppDataDirectory/BasicDrawingApp`.
- Status shows: `Saved to app documents: filename.bdraw (...) Use Load .bdraw > Load from App Files.`
- `Load .bdraw` opens an action sheet:
  - `Load from App Files` lists `.bdraw` files saved by the app.
  - `Import .bdraw` opens Android FilePicker and reads the picked file stream.
- `Export PNG` writes `image/png` to MediaStore under `Pictures/BasicDrawingApp`.
- `Export JPEG` writes `image/jpeg` to MediaStore under `Pictures/BasicDrawingApp`.
- Status shows `Exported to Photos/Gallery: filename.png in Pictures/BasicDrawingApp` or `.jpg`.
- No `MANAGE_EXTERNAL_STORAGE` permission is requested.

Manual Android emulator/device execution was not run from this terminal session, so Photos/Gallery visibility and opening exported images still need device confirmation.

## `.bdraw` Format Checks

- Magic header: `BDRAW`.
- Version: `1`.
- Shape count is written and rejected if negative.
- Each shape stores id, kind, start/end points, stroke color RGBA, fill color RGBA, thickness, fill flag, and creation ticks.
- Incomplete files throw `InvalidDataException` with a clear message.
- Wrong magic or unsupported version throws `InvalidDataException` with a clear message.

## Manual Android Checklist To Complete

1. Draw Point, Line, Rectangle, Square, Ellipse, and Circle.
2. Use different stroke colors, fill colors, `No Fill`, and thickness.
3. Tap `Save .bdraw to App Files`; confirm filename and app documents status.
4. Confirm the file exists through app load list.
5. Clear canvas.
6. Tap `Load .bdraw` > `Load from App Files`; select the saved file.
7. Confirm all shapes, positions, colors, fill state, thickness, and count restore.
8. Draw one more shape after load.
9. Save again.
10. Export PNG and confirm it appears in Photos/Gallery under `Pictures/BasicDrawingApp`.
11. Open the PNG and confirm it is not blank, has a white background, and includes all shapes.
12. Export JPEG and confirm it appears in Photos/Gallery under `Pictures/BasicDrawingApp`.
13. Open the JPEG and confirm it is not 0 bytes, has a white background, and includes all shapes.
