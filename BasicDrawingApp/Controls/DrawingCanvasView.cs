using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using BasicDrawingApp.Models;

namespace BasicDrawingApp.Controls;

public sealed class DrawingCanvasView : GraphicsView
{
    public static readonly BindableProperty DocumentProperty =
        BindableProperty.Create(nameof(Document), typeof(DrawingDocument), typeof(DrawingCanvasView), propertyChanged: OnDocumentChanged);

    public static readonly BindableProperty ShapesProperty =
        BindableProperty.Create(nameof(Shapes), typeof(ObservableCollection<DrawingShape>), typeof(DrawingCanvasView), propertyChanged: OnShapesChanged);

    public static readonly BindableProperty PreviewShapeProperty =
        BindableProperty.Create(nameof(PreviewShape), typeof(DrawingShape), typeof(DrawingCanvasView), defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnDrawingPropertyChanged);

    public static readonly BindableProperty SelectedToolProperty =
        BindableProperty.Create(nameof(SelectedTool), typeof(ShapeKind), typeof(DrawingCanvasView), ShapeKind.Line);

    public static readonly BindableProperty SelectedStrokeColorProperty =
        BindableProperty.Create(nameof(SelectedStrokeColor), typeof(Color), typeof(DrawingCanvasView), Colors.Black);

    public static readonly BindableProperty SelectedFillColorProperty =
        BindableProperty.Create(nameof(SelectedFillColor), typeof(Color), typeof(DrawingCanvasView), Colors.Transparent);

    public static readonly BindableProperty StrokeThicknessProperty =
        BindableProperty.Create(nameof(StrokeThickness), typeof(double), typeof(DrawingCanvasView), 2d);

    public static readonly BindableProperty IsFillEnabledProperty =
        BindableProperty.Create(nameof(IsFillEnabled), typeof(bool), typeof(DrawingCanvasView), false);

    public static readonly BindableProperty AddShapeCommandProperty =
        BindableProperty.Create(nameof(AddShapeCommand), typeof(ICommand), typeof(DrawingCanvasView));

    public DrawingCanvasView()
    {
        Drawable = new DrawingCanvasDrawable(this);
        BackgroundColor = Colors.White;
        StartInteraction += OnStartInteraction;
        DragInteraction += OnDragInteraction;
        EndInteraction += OnEndInteraction;
        CancelInteraction += (_, _) => ClearPreview();
    }

    public DrawingDocument? Document
    {
        get => (DrawingDocument?)GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value);
    }

    public ObservableCollection<DrawingShape>? Shapes
    {
        get => (ObservableCollection<DrawingShape>?)GetValue(ShapesProperty);
        set => SetValue(ShapesProperty, value);
    }

    public DrawingShape? PreviewShape
    {
        get => (DrawingShape?)GetValue(PreviewShapeProperty);
        set => SetValue(PreviewShapeProperty, value);
    }

    public ShapeKind SelectedTool
    {
        get => (ShapeKind)GetValue(SelectedToolProperty);
        set => SetValue(SelectedToolProperty, value);
    }

    public Color SelectedStrokeColor
    {
        get => (Color)GetValue(SelectedStrokeColorProperty);
        set => SetValue(SelectedStrokeColorProperty, value);
    }

    public Color SelectedFillColor
    {
        get => (Color)GetValue(SelectedFillColorProperty);
        set => SetValue(SelectedFillColorProperty, value);
    }

    public double StrokeThickness
    {
        get => (double)GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public bool IsFillEnabled
    {
        get => (bool)GetValue(IsFillEnabledProperty);
        set => SetValue(IsFillEnabledProperty, value);
    }

    public ICommand? AddShapeCommand
    {
        get => (ICommand?)GetValue(AddShapeCommandProperty);
        set => SetValue(AddShapeCommandProperty, value);
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (Document is null || width <= 0 || height <= 0)
        {
            return;
        }

        Document.CanvasWidth = (float)width;
        Document.CanvasHeight = (float)height;
    }

    private static void OnDocumentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        DrawingCanvasView view = (DrawingCanvasView)bindable;
        view.Invalidate();
    }

    private static void OnShapesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        DrawingCanvasView view = (DrawingCanvasView)bindable;

        if (oldValue is INotifyCollectionChanged oldCollection)
        {
            oldCollection.CollectionChanged -= view.OnShapesCollectionChanged;
        }

        if (newValue is INotifyCollectionChanged newCollection)
        {
            newCollection.CollectionChanged += view.OnShapesCollectionChanged;
        }

        view.Invalidate();
    }

    private static void OnDrawingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((DrawingCanvasView)bindable).Invalidate();
    }

    private void OnShapesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Invalidate();
    }

    private void OnStartInteraction(object? sender, TouchEventArgs e)
    {
        PointF point = ClampPoint(e.Touches[0]);
        PreviewShape = CreateShape(point, point);

        if (SelectedTool == ShapeKind.Point)
        {
            CommitPreview();
            return;
        }

        Invalidate();
    }

    private void OnDragInteraction(object? sender, TouchEventArgs e)
    {
        if (PreviewShape is null)
        {
            return;
        }

        DrawingShape shape = PreviewShape.Clone();
        shape.EndPoint = ClampPoint(e.Touches[0]);
        PreviewShape = shape;
        Invalidate();
    }

    private void OnEndInteraction(object? sender, TouchEventArgs e)
    {
        if (PreviewShape is null)
        {
            return;
        }

        DrawingShape shape = PreviewShape.Clone();
        shape.EndPoint = ClampPoint(e.Touches[0]);
        PreviewShape = shape;
        CommitPreview();
    }

    private DrawingShape CreateShape(PointF start, PointF end)
    {
        return new DrawingShape
        {
            Kind = SelectedTool,
            StartPoint = start,
            EndPoint = end,
            StrokeColor = SelectedStrokeColor,
            FillColor = SelectedFillColor,
            StrokeThickness = (float)StrokeThickness,
            IsFilled = IsFillEnabled && SelectedTool is not ShapeKind.Line,
            CreatedAt = DateTime.UtcNow
        };
    }

    private void CommitPreview()
    {
        if (PreviewShape is null)
        {
            return;
        }

        DrawingShape shape = PreviewShape.Clone();
        PreviewShape = null;

        if (AddShapeCommand?.CanExecute(shape) == true)
        {
            AddShapeCommand.Execute(shape);
        }

        Invalidate();
    }

    private void ClearPreview()
    {
        PreviewShape = null;
        Invalidate();
    }

    private PointF ClampPoint(PointF point)
    {
        float x = Math.Clamp(point.X, 0, Math.Max(0, (float)Width));
        float y = Math.Clamp(point.Y, 0, Math.Max(0, (float)Height));
        return new PointF(x, y);
    }

    private sealed class DrawingCanvasDrawable : IDrawable
    {
        private readonly DrawingCanvasView _view;

        public DrawingCanvasDrawable(DrawingCanvasView view)
        {
            _view = view;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            DrawingDocument document = _view.Document ?? new DrawingDocument
            {
                CanvasWidth = dirtyRect.Width,
                CanvasHeight = dirtyRect.Height
            };

            DrawingRenderer.DrawDocument(canvas, document, _view.PreviewShape, dirtyRect);
        }
    }
}
