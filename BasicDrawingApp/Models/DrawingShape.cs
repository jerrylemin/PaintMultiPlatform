using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BasicDrawingApp.Models;

public sealed class DrawingShape : INotifyPropertyChanged
{
    private string _id = Guid.NewGuid().ToString("N");
    private ShapeKind _kind;
    private PointF _startPoint;
    private PointF _endPoint;
    private Color _strokeColor = Colors.Black;
    private Color _fillColor = Colors.Transparent;
    private float _strokeThickness = 2;
    private bool _isFilled;
    private DateTime _createdAt = DateTime.UtcNow;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public ShapeKind Kind
    {
        get => _kind;
        set => SetProperty(ref _kind, value);
    }

    public PointF StartPoint
    {
        get => _startPoint;
        set => SetProperty(ref _startPoint, value);
    }

    public PointF EndPoint
    {
        get => _endPoint;
        set => SetProperty(ref _endPoint, value);
    }

    public Color StrokeColor
    {
        get => _strokeColor;
        set => SetProperty(ref _strokeColor, value);
    }

    public Color FillColor
    {
        get => _fillColor;
        set => SetProperty(ref _fillColor, value);
    }

    public float StrokeThickness
    {
        get => _strokeThickness;
        set => SetProperty(ref _strokeThickness, value);
    }

    public bool IsFilled
    {
        get => _isFilled;
        set => SetProperty(ref _isFilled, value);
    }

    public DateTime CreatedAt
    {
        get => _createdAt;
        set => SetProperty(ref _createdAt, value);
    }

    public DrawingShape Clone()
    {
        return new DrawingShape
        {
            Id = Id,
            Kind = Kind,
            StartPoint = StartPoint,
            EndPoint = EndPoint,
            StrokeColor = StrokeColor,
            FillColor = FillColor,
            StrokeThickness = StrokeThickness,
            IsFilled = IsFilled,
            CreatedAt = CreatedAt
        };
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
