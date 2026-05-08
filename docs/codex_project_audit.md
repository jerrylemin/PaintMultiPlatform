# Codex Project Audit

## Current State Before This Fix

- The project already existed as a .NET MAUI app named `BasicDrawingApp`.
- The app compiled, but the drawing experience was not acceptable for grading.
- The toolbar used `Picker` controls for tool/color selection, so the shape tools were not obvious on Windows or Android.
- The default selected tool was `Line`, and users could easily miss the hidden picker choices. This made the app appear to only draw lines.
- `DrawingCanvasView` did not subscribe to `ObservableCollection<DrawingShape>.CollectionChanged`, so redraw after add/undo/redo/load was not reliable.
- Preview shape was stored only inside the canvas control, not in `MainViewModel`, so it did not match the required MVVM state.

## Why It Looked Like Only Line Worked

- `MainViewModel.SelectedTool` defaulted to `ShapeKind.Line`.
- The old UI hid tool selection inside a compact `Picker`; there were no clear buttons for Point, Rectangle, Square, Ellipse, or Circle.
- The canvas only invalidated during interaction and did not listen to shape collection changes, so committed non-line shapes could fail to appear clearly after release.
- Preview state lived in the control instead of the ViewModel, making it harder to reason about redraw and state updates.

## Missing Or Weak Features

- Visible segmented-style tool buttons were missing.
- Visible fill/stroke color buttons were missing.
- The status area did not show enough explicit current state.
- Canvas redraw was not wired directly to `Shapes` and `PreviewShape`.
- Mobile layout existed but the toolbar was not clear enough for touch use.

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

- Replaced hidden tool/color pickers with explicit buttons.
- Added `Shapes` and `PreviewShape` bindings to the canvas.
- Made canvas subscribe to shape collection changes so every add/undo/redo/load redraws.
- Kept `GraphicsView` for live drawing and SkiaSharp for image export.
- Kept Windows and Android target frameworks only.

## Current Known Notes

- The active command line SDK is .NET 10 preview, so builds print `NETSDK1057`. The project still targets .NET 9.
- The workspace is not a git repository, so requested commits could not be created.
