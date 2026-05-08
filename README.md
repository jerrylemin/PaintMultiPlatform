# Basic Drawing App

Basic Drawing App is a .NET MAUI drawing application for Windows and Android from one shared C# codebase.

## Platform

- Framework: .NET MAUI
- Targets: Windows and Android
- Main project: `BasicDrawingApp/BasicDrawingApp.csproj`

.NET MAUI was selected because it supports Windows, Android, iOS, and macOS from a shared C# codebase. This project focuses on Windows and Android for the assignment.

## Team

- 21127645 - Le Minh - Developer
- 21127224 - Nguyen Vu Bach - Developer

Submission placeholders:

- Windows demo:
- Android demo:

Demo video requirements: Unlisted, under 5 minutes, no voiceover, no music.

## Features

- Draw Point, Line, Rectangle, Square, Ellipse, and Circle.
- Preview the selected shape while dragging.
- Select stroke color: Black, Red, Green, Blue, Yellow, Purple, Orange.
- Select fill color: Transparent, Black, Red, Green, Blue, Yellow, Purple, Orange.
- Enable or disable fill.
- Select stroke thickness from 1 to 20.
- Undo, redo, and clear canvas.
- Save all shapes to a custom binary `.bdraw` file.
- Load a `.bdraw` file and continue drawing.
- Export the full white canvas to PNG or JPEG.
- Responsive layout for Windows desktop and Android screens.

## How To Open In Visual Studio

1. Open Visual Studio with .NET MAUI workload installed.
2. Choose `Open a project or solution`.
3. Open `BasicDrawingApp/BasicDrawingApp.csproj`.
4. Select `Windows Machine` to run on Windows.
5. Select an Android emulator or Android device to run on Android.

## Run From Terminal

Windows:

```powershell
dotnet build BasicDrawingApp\BasicDrawingApp.csproj -f net9.0-windows10.0.19041.0
dotnet run --project BasicDrawingApp\BasicDrawingApp.csproj -f net9.0-windows10.0.19041.0
```

Android emulator/device:

```powershell
dotnet build BasicDrawingApp\BasicDrawingApp.csproj -f net9.0-android
dotnet build BasicDrawingApp\BasicDrawingApp.csproj -f net9.0-android -t:Run
```

## How To Draw

1. Select a tool button: `Point`, `Line`, `Rectangle`, `Square`, `Ellipse`, or `Circle`.
2. Select stroke color, fill color, fill enabled, and stroke thickness.
3. Click/tap once for `Point`.
4. Drag from start to end for all other shapes.
5. Release the mouse/finger to commit the preview shape to the canvas.

Shape behavior:

- Point: draws a small circle based on stroke thickness.
- Line: draws from start point to end point.
- Rectangle: uses the dragged bounding box.
- Square: uses `min(width, height)` and keeps drag direction.
- Ellipse: uses the dragged bounding box.
- Circle: uses a square bounding box so the shape is not distorted.

## Save And Load `.bdraw`

- `Save .bdraw` writes the current canvas to app data under a `Drawings` folder.
- `Load .bdraw` opens a file picker and loads a binary drawing file.
- Invalid format or unsupported version is reported in the status text without crashing.

Binary format:

- String magic: `BDRAW`
- Int32 version: `1`
- Single canvas width
- Single canvas height
- Int32 shape count
- Per shape: id, kind, start/end coordinates, stroke RGBA, fill RGBA, thickness, fill flag, created ticks

## Export PNG/JPEG

- `Export PNG` writes `drawing_yyyyMMdd_HHmmss.png`.
- `Export JPEG` writes `drawing_yyyyMMdd_HHmmss.jpg`.
- Windows exports to `Pictures/BasicDrawingApp` when available.
- Android exports to the app data folder and opens the share sheet.

## GitHub Notes

- Do not commit personal access tokens.
- If the teacher needs a token for a private repo, create a read-only token, set an expiration date, and send it privately.
