# Basic Drawing App

Basic Drawing App is a .NET MAUI drawing application for Windows and Android from one shared C# codebase.

## Platform

- Framework: .NET MAUI
- Targets: Windows and Android
- Project: `BasicDrawingApp/BasicDrawingApp.csproj`

## Team

- 21127645 - Le Minh - Developer
- 21127224 - Nguyen Vu Bach - Developer

Demo placeholders:

- Windows demo:
- Android demo:

Video requirement: Unlisted, under 5 minutes, no voiceover, no music.

## Interface

The app uses a light Paint-inspired workspace without copying Paint directly:

- Header: app name and current status.
- Toolbar: grouped ribbon-style controls for Tools, Stroke, Fill, Thickness, and Actions.
- Canvas: a large white drawing surface with a clear border.

The toolbar scrolls horizontally on smaller screens, so Android users can swipe through all controls.

## Features

- Draw Point, Line, Rectangle, Square, Ellipse, and Circle.
- Preview while dragging.
- Stroke color applies to every shape.
- Fill color applies only to Rectangle, Square, Ellipse, and Circle.
- `No Fill` means transparent fill; closed shapes draw outline only.
- Point uses stroke color and thickness for a visible dot.
- Line ignores fill.
- Stroke thickness range: 1 to 20 px, with live preview.
- Undo, redo, clear canvas.
- Save and load custom `.bdraw` binary drawings.
- Export PNG and JPEG.

## How To Draw

1. Pick a tool from the Tools group.
2. Pick a stroke color from the Stroke swatches.
3. For closed shapes, pick a fill color or `No Fill`, then enable Fill.
4. Adjust Thickness.
5. Tap once for Point, or drag for Line/Rectangle/Square/Ellipse/Circle.

If a selected fill color does not appear, check that:

- Fill is enabled.
- The shape is Rectangle, Square, Ellipse, or Circle.
- Fill is not set to `No Fill`.

## Save And Load `.bdraw`

- Windows: `Save .bdraw` opens a save picker so you can choose the folder and filename.
- Windows: `Load .bdraw` opens a file picker for existing `.bdraw` files.
- Android: `Save .bdraw to App Files` saves to the app-private documents folder under `FileSystem.AppDataDirectory/BasicDrawingApp` with a filename like `drawing_yyyyMMdd_HHmmss.bdraw`.
- Android: saved `.bdraw` files are private app files, so they do not appear in Android Photos/Gallery. Use `Load .bdraw` then choose `Load from App Files` to reopen drawings saved by the app.
- Android: `Load .bdraw` also offers `Import .bdraw`, which opens Android FilePicker for `.bdraw` files stored outside the app.
- `.bdraw` is a custom binary format using `BinaryWriter` and `BinaryReader`.

## Export PNG/JPEG

- Windows: `Export PNG` and `Export JPEG` open a save picker.
- Android: `Export PNG` and `Export JPEG` write directly to Android MediaStore as `image/png` or `image/jpeg`.
- Android: exported images appear in Photos/Gallery under `Pictures/BasicDrawingApp` with filenames like `drawing_yyyyMMdd_HHmmss.png` or `.jpg`.
- Exported images use a white background and include the full canvas.

## Android Quick Demo Test

1. Open the app on an Android emulator or device.
2. Draw Point, Line, Rectangle, Square, Ellipse, and Circle.
3. Tap `Save .bdraw to App Files`; confirm the status shows the saved filename and app documents location.
4. Tap `Clear`, then `Load .bdraw` > `Load from App Files`, and choose the saved file.
5. Confirm the shape count and drawing are restored, then draw one more shape.
6. Tap `Export PNG`, open Photos/Gallery, and check `Pictures/BasicDrawingApp`.
7. Tap `Export JPEG`, open Photos/Gallery, and check the JPEG opens normally.

## Run In Visual Studio

1. Open Visual Studio with the .NET MAUI workload installed.
2. Open `BasicDrawingApp/BasicDrawingApp.csproj`.
3. Choose `Windows Machine` for Windows.
4. Choose an Android emulator/device for Android.
5. Press Run.

## Run From Terminal

```powershell
dotnet build BasicDrawingApp\BasicDrawingApp.csproj
dotnet run --project BasicDrawingApp\BasicDrawingApp.csproj -f net9.0-windows10.0.19041.0
```

Android:

```powershell
dotnet build BasicDrawingApp\BasicDrawingApp.csproj -f net9.0-android
dotnet build BasicDrawingApp\BasicDrawingApp.csproj -f net9.0-android -t:Run
```

## GitHub Notes

- Do not commit personal access tokens.
- If a private repository needs a grading token, create a read-only token with expiration and send it privately.
