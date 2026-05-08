# Codex Architecture Plan

## Architecture

The app is organized as a small MVVM MAUI project:

- `Models`: drawing data objects.
- `ViewModels`: selected tool, colors, thickness, preview shape, shape collection, and commands.
- `Views`: XAML layout and responsive desktop/mobile switching.
- `Controls`: reusable drawing canvas and render helper.
- `Services`: binary serialization, image export, and file path/picker handling.

## Main Flow

1. User taps a toolbar button in `MainPage.xaml`.
2. `MainViewModel` updates `SelectedTool`, `SelectedStrokeColor`, `SelectedFillColor`, `StrokeThickness`, or `IsFillEnabled`.
3. `DrawingCanvasView` receives bound state.
4. On pointer/touch start and drag, canvas creates or updates `PreviewShape`.
5. `PreviewShape` is bound back to `MainViewModel`.
6. On release, canvas sends a `DrawingShape` to `AddShapeCommand`.
7. `MainViewModel.Shapes` changes and the canvas redraws through its `CollectionChanged` subscription.

## Render Plan

- `DrawingRenderer.DrawShape(ICanvas, DrawingShape)` handles live MAUI drawing.
- `DrawingRenderer.DrawShape(SKCanvas, DrawingShape)` handles export drawing.
- Point, Line, Rectangle, Square, Ellipse, and Circle are all handled by `switch (shape.Kind)`.
- Square and Circle use `GetSquareRect`, based on `min(abs(dx), abs(dy))`.

## Save/Load Plan

`DrawingBinarySerializer` uses `BinaryWriter` and `BinaryReader`.

Format:

- String magic: `BDRAW`
- Int32 version: `1`
- Single canvas width
- Single canvas height
- Int32 shape count
- Per shape: id, kind, startX, startY, endX, endY, stroke RGBA, fill RGBA, thickness, isFilled, created ticks

Invalid magic, unsupported version, negative count, or truncated file becomes `InvalidDataException`.

## Export Plan

`ImageExportService` renders the full document to an `SKBitmap` with a white background and writes:

- PNG quality 100
- JPEG quality 90

Windows uses Pictures/AppData. Android uses app data and share sheet.
