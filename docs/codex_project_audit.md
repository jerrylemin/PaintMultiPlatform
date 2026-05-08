# Codex Project Audit

## Current State Before This Fix

- The project already existed as a .NET MAUI app named `BasicDrawingApp`.
- The app compiled, but the drawing experience was not acceptable for grading.
- The toolbar had been changed to visible controls, but it still lived in the same `Grid` row as the canvas on desktop: `RowDefinitions="Auto,*"` and `ColumnDefinitions="340,*"`.
- `ToolScroll` was placed at `Grid.Row="1"` and the canvas was also placed at `Grid.Row="1"` in another column. On wide Windows layouts, this made the toolbar a side panel. In the user's run, the visible area showed header/status and the canvas, while the left toolbar column was effectively not visible.
- The old `MainPage.xaml.cs` moved toolbar/canvas only when `Width < 760`; otherwise it kept the side-panel layout. This was the real layout reason the user saw only title, status, and white canvas.
- Toolbar was not `IsVisible=false`.
- Toolbar commands were bound to `MainViewModel`.
- Toolbar did not have a tiny `HeightRequest`; it was misplaced in the desktop grid structure.

## Why It Looked Like Only Line Worked

- `MainViewModel.SelectedTool` defaulted to `ShapeKind.Line`.
- Because the toolbar was not visible in the user's app window, the user could not press Point, Rectangle, Square, Ellipse, or Circle.
- Canvas received the default `SelectedTool`, so drag gestures created lines.

## Missing Or Weak Features

- Toolbar needed to be a guaranteed visible row, not a side column competing with the canvas.
- Tool buttons needed selected-state visual feedback.
- Android needed a scrollable toolbar to avoid clipping.

## Responsibility Map

- Canvas interaction: `BasicDrawingApp/Controls/DrawingCanvasView.cs`
- Rendering helper: `BasicDrawingApp/Controls/DrawingRenderer.cs`
- Toolbar UI: `BasicDrawingApp/Views/MainPage.xaml`
- Responsive layout switching: `BasicDrawingApp/Views/MainPage.xaml.cs`
- App state and commands: `BasicDrawingApp/ViewModels/MainViewModel.cs`
- Models: `BasicDrawingApp/Models/DrawingShape.cs`, `ShapeKind.cs`, `DrawingDocument.cs`
- Binary save/load: `BasicDrawingApp/Services/DrawingBinarySerializer.cs`
- Export PNG/JPEG: `BasicDrawingApp/Services/ImageExportService.cs`
- File paths and picker: `BasicDrawingApp/Services/FilePickerService.cs`

## Decisions In This Fix

- Rebuilt `MainPage.xaml` as exactly three rows: header, toolbar, canvas.
- Put toolbar in `Grid.Row="1"` and canvas in `Grid.Row="2"` so they cannot overlap or hide each other.
- Wrapped toolbar in a horizontal `ScrollView`; on Windows it is immediately visible, and on Android it can scroll instead of overflowing.
- Added selected tool background bindings in `MainViewModel`.
- Removed the old size-changing row/column movement logic from `MainPage.xaml.cs`.
- Kept existing `GraphicsView` canvas, binary serializer, and SkiaSharp export services.

## Current Known Notes

- The active command line SDK is .NET 10 preview, so builds print `NETSDK1057`. The project still targets .NET 9.
- The workspace is not a git repository, so requested commits could not be created.
