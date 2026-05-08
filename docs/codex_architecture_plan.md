# Codex Architecture Plan

## Current Architecture

The app stays in a simple MVVM structure:

- `Models`: `DrawingShape`, `ShapeKind`, `DrawingDocument`.
- `ViewModels`: `MainViewModel`, commands, selected tool/color/thickness/fill state.
- `Views`: `MainPage.xaml` and `MainPage.xaml.cs`.
- `Controls`: `DrawingCanvasView` and `DrawingRenderer`.
- `Services`: `DrawingBinarySerializer`, `FilePickerService`, `ImageExportService`.

## UI Direction

The visual direction is a light, modern drawing workspace inspired by the organization of Paint-style apps, without copying Paint directly.

The page has:

- Header: app title, short workspace label, status summary.
- Toolbar: horizontally scrollable ribbon with grouped tools.
- Canvas: framed white drawing surface on a quiet neutral background.

Toolbar groups:

- Tools: Point, Line, Rectangle, Square, Ellipse, Circle.
- Stroke: compact color swatches with selected border.
- Fill: `No Fill` plus color swatches with selected border.
- Thickness: slider, `px` value, and live line preview.
- Actions: undo, redo, clear, save/load, export PNG/JPEG.

## Fill And Stroke Rules

- Stroke color applies to all shapes, including point and line.
- Fill color applies only to Rectangle, Square, Ellipse, and Circle.
- `No Fill` maps to `Colors.Transparent`.
- Fill only appears when `IsFillEnabled` is true and selected fill is not `No Fill`.
- Point is rendered as a visible dot based on stroke color/thickness.

## File And Export Plan

- `.bdraw` save/load stays binary through `DrawingBinarySerializer`.
- Windows save/export uses `FileSavePicker` so users can browse and choose a local path.
- Windows load uses MAUI `FilePicker`.
- Android `.bdraw` save uses app storage.
- Android image export writes to app storage, publishes to MediaStore Pictures/BasicDrawingApp when supported, and opens share sheet.

## Build Target

- `net9.0-windows10.0.19041.0`
- `net9.0-android`
