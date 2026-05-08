using System.Collections.ObjectModel;
using System.Windows.Input;
using BasicDrawingApp.Models;
using BasicDrawingApp.Services;

namespace BasicDrawingApp.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly DrawingBinarySerializer _serializer;
    private readonly ImageExportService _imageExportService;
    private readonly FilePickerService _filePickerService;
    private readonly Stack<DrawingShape> _undoStack = [];
    private readonly Stack<DrawingShape> _redoStack = [];
    private DrawingDocument _document = new();
    private ShapeKind _selectedTool = ShapeKind.Line;
    private Color _selectedStrokeColor = Colors.Black;
    private Color _selectedFillColor = Colors.Transparent;
    private double _strokeThickness = 3;
    private bool _isFillEnabled;
    private DrawingShape? _previewShape;
    private string _statusMessage = "Current tool: Line | Stroke: Black | Fill: Transparent | Thickness: 3 | Shapes: 0 | Last action: Ready";
    private string _lastAction = "Ready";

    public MainViewModel(
        DrawingBinarySerializer serializer,
        ImageExportService imageExportService,
        FilePickerService filePickerService)
    {
        _serializer = serializer;
        _imageExportService = imageExportService;
        _filePickerService = filePickerService;

        ToolOptions =
        [
            ShapeKind.Point,
            ShapeKind.Line,
            ShapeKind.Rectangle,
            ShapeKind.Square,
            ShapeKind.Ellipse,
            ShapeKind.Circle
        ];

        StrokeColors =
        [
            new("Black", Colors.Black),
            new("Red", Colors.Red),
            new("Green", Colors.Green),
            new("Blue", Colors.Blue),
            new("Yellow", Colors.Yellow),
            new("Purple", Colors.Purple),
            new("Orange", Colors.Orange)
        ];

        FillColors =
        [
            new("Transparent", Colors.Transparent),
            new("Black", Colors.Black),
            new("Red", Colors.Red),
            new("Green", Colors.Green),
            new("Blue", Colors.Blue),
            new("Yellow", Colors.Yellow),
            new("Purple", Colors.Purple),
            new("Orange", Colors.Orange)
        ];

        SelectToolCommand = new RelayCommand(SelectTool);
        SelectStrokeColorCommand = new RelayCommand(SelectStrokeColor);
        SelectFillColorCommand = new RelayCommand(SelectFillColor);
        AddShapeCommand = new RelayCommand(AddShape);
        ClearCommand = new RelayCommand(Clear);
        UndoCommand = new RelayCommand(Undo);
        RedoCommand = new RelayCommand(Redo);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        LoadCommand = new AsyncRelayCommand(LoadAsync);
        ExportPngCommand = new AsyncRelayCommand(ExportPngAsync);
        ExportJpegCommand = new AsyncRelayCommand(ExportJpegAsync);
    }

    public IReadOnlyList<ShapeKind> ToolOptions { get; }

    public ObservableCollection<DrawingColorOption> StrokeColors { get; }

    public ObservableCollection<DrawingColorOption> FillColors { get; }

    public DrawingDocument Document
    {
        get => _document;
        private set
        {
            if (SetProperty(ref _document, value))
            {
                OnPropertyChanged(nameof(Shapes));
                OnPropertyChanged(nameof(ShapeCount));
            }
        }
    }

    public ObservableCollection<DrawingShape> Shapes => Document.Shapes;

    public ShapeKind SelectedTool
    {
        get => _selectedTool;
        set
        {
            if (SetProperty(ref _selectedTool, value))
            {
                OnPropertyChanged(nameof(PointToolBackground));
                OnPropertyChanged(nameof(LineToolBackground));
                OnPropertyChanged(nameof(RectangleToolBackground));
                OnPropertyChanged(nameof(SquareToolBackground));
                OnPropertyChanged(nameof(EllipseToolBackground));
                OnPropertyChanged(nameof(CircleToolBackground));
                SetLastAction($"Selected tool {SelectedTool}");
            }
        }
    }

    public Color SelectedStrokeColor
    {
        get => _selectedStrokeColor;
        set
        {
            if (SetProperty(ref _selectedStrokeColor, value))
            {
                OnPropertyChanged(nameof(StrokeColorName));
                SetLastAction($"Selected stroke {StrokeColorName}");
            }
        }
    }

    public Color SelectedFillColor
    {
        get => _selectedFillColor;
        set
        {
            if (SetProperty(ref _selectedFillColor, value))
            {
                OnPropertyChanged(nameof(FillColorName));
                SetLastAction($"Selected fill {FillColorName}");
            }
        }
    }

    public double StrokeThickness
    {
        get => _strokeThickness;
        set
        {
            double normalizedValue = Math.Clamp(value, 1, 20);
            if (SetProperty(ref _strokeThickness, normalizedValue))
            {
                OnPropertyChanged(nameof(StrokeThicknessText));
                SetLastAction($"Stroke thickness {StrokeThicknessText}");
            }
        }
    }

    public string StrokeThicknessText => $"{StrokeThickness:0}";

    public bool IsFillEnabled
    {
        get => _isFillEnabled;
        set
        {
            if (SetProperty(ref _isFillEnabled, value))
            {
                SetLastAction(IsFillEnabled ? "Fill enabled" : "Fill disabled");
            }
        }
    }

    public DrawingShape? PreviewShape
    {
        get => _previewShape;
        set => SetProperty(ref _previewShape, value);
    }

    public int ShapeCount => Shapes.Count;

    public string StrokeColorName => GetColorName(StrokeColors, SelectedStrokeColor);

    public string FillColorName => GetColorName(FillColors, SelectedFillColor);

    public Color PointToolBackground => GetToolBackground(ShapeKind.Point);

    public Color LineToolBackground => GetToolBackground(ShapeKind.Line);

    public Color RectangleToolBackground => GetToolBackground(ShapeKind.Rectangle);

    public Color SquareToolBackground => GetToolBackground(ShapeKind.Square);

    public Color EllipseToolBackground => GetToolBackground(ShapeKind.Ellipse);

    public Color CircleToolBackground => GetToolBackground(ShapeKind.Circle);

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public ICommand SelectToolCommand { get; }

    public ICommand SelectStrokeColorCommand { get; }

    public ICommand SelectFillColorCommand { get; }

    public ICommand AddShapeCommand { get; }

    public ICommand ClearCommand { get; }

    public ICommand UndoCommand { get; }

    public ICommand RedoCommand { get; }

    public ICommand SaveCommand { get; }

    public ICommand LoadCommand { get; }

    public ICommand ExportPngCommand { get; }

    public ICommand ExportJpegCommand { get; }

    private void SelectTool(object? parameter)
    {
        if (parameter is ShapeKind kind)
        {
            SelectedTool = kind;
            return;
        }

        if (parameter is string text && Enum.TryParse(text, out ShapeKind parsedKind))
        {
            SelectedTool = parsedKind;
        }
    }

    private void SelectStrokeColor(object? parameter)
    {
        if (TryGetColor(parameter, StrokeColors, out Color color))
        {
            SelectedStrokeColor = color;
        }
    }

    private void SelectFillColor(object? parameter)
    {
        if (TryGetColor(parameter, FillColors, out Color color))
        {
            SelectedFillColor = color;
        }
    }

    private void AddShape(object? parameter)
    {
        if (parameter is not DrawingShape shape)
        {
            return;
        }

        Shapes.Add(shape);
        Document.Touch();
        PreviewShape = null;
        _undoStack.Push(shape);
        _redoStack.Clear();
        OnDocumentChanged($"Added {shape.Kind}");
    }

    private void Clear()
    {
        Shapes.Clear();
        Document.Touch();
        PreviewShape = null;
        _undoStack.Clear();
        _redoStack.Clear();
        OnDocumentChanged("Canvas cleared");
    }

    private void Undo()
    {
        if (_undoStack.Count == 0)
        {
            SetLastAction("Nothing to undo");
            return;
        }

        DrawingShape shape = _undoStack.Pop();
        if (Shapes.Remove(shape))
        {
            _redoStack.Push(shape);
            Document.Touch();
            PreviewShape = null;
            OnDocumentChanged("Undo complete");
        }
    }

    private void Redo()
    {
        if (_redoStack.Count == 0)
        {
            SetLastAction("Nothing to redo");
            return;
        }

        DrawingShape shape = _redoStack.Pop();
        Shapes.Add(shape);
        _undoStack.Push(shape);
        Document.Touch();
        PreviewShape = null;
        OnDocumentChanged("Redo complete");
    }

    private async Task SaveAsync()
    {
        try
        {
            string path = await _filePickerService.CreateSavePathAsync();
            await _serializer.SaveAsync(Document, path);
            SetLastAction($"Saved {path}");
        }
        catch (Exception ex)
        {
            SetLastAction($"Save failed: {ex.Message}");
        }
    }

    private async Task LoadAsync()
    {
        try
        {
            string? path = await _filePickerService.PickBdrawFileAsync();
            if (string.IsNullOrWhiteSpace(path))
            {
                SetLastAction("Load canceled");
                return;
            }

            Document = await _serializer.LoadAsync(path);
            PreviewShape = null;
            _undoStack.Clear();
            _redoStack.Clear();
            OnDocumentChanged($"Loaded {path}");
        }
        catch (InvalidDataException ex)
        {
            SetLastAction($"Invalid .bdraw file: {ex.Message}");
        }
        catch (Exception ex)
        {
            SetLastAction($"Load failed: {ex.Message}");
        }
    }

    private async Task ExportPngAsync()
    {
        await ExportAsync(isPng: true);
    }

    private async Task ExportJpegAsync()
    {
        await ExportAsync(isPng: false);
    }

    private async Task ExportAsync(bool isPng)
    {
        try
        {
            string path = isPng
                ? await _imageExportService.ExportPngAsync(Document)
                : await _imageExportService.ExportJpegAsync(Document);
            SetLastAction($"Exported {path}");
        }
        catch (Exception ex)
        {
            SetLastAction($"Export failed: {ex.Message}");
        }
    }

    private void OnDocumentChanged(string lastAction)
    {
        OnPropertyChanged(nameof(ShapeCount));
        SetLastAction(lastAction);
    }

    private void SetLastAction(string action)
    {
        _lastAction = action;
        StatusMessage = $"Current tool: {SelectedTool} | Stroke: {StrokeColorName} | Fill: {FillColorName} | Stroke thickness: {StrokeThicknessText} | Shape count: {ShapeCount} | Last action: {_lastAction}";
    }

    private static bool TryGetColor(object? parameter, IEnumerable<DrawingColorOption> options, out Color color)
    {
        if (parameter is Color parameterColor)
        {
            color = parameterColor;
            return true;
        }

        if (parameter is string colorName)
        {
            DrawingColorOption? option = options.FirstOrDefault(item => string.Equals(item.Name, colorName, StringComparison.OrdinalIgnoreCase));
            if (option is not null)
            {
                color = option.Value;
                return true;
            }
        }

        color = Colors.Transparent;
        return false;
    }

    private static string GetColorName(IEnumerable<DrawingColorOption> options, Color color)
    {
        DrawingColorOption? option = options.FirstOrDefault(item => item.Value == color);
        return option?.Name ?? color.ToArgbHex();
    }

    private Color GetToolBackground(ShapeKind kind)
    {
        return SelectedTool == kind ? Color.FromArgb("#EA580C") : Color.FromArgb("#475569");
    }
}
