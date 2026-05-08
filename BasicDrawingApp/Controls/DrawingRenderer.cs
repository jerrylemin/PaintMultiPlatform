using BasicDrawingApp.Models;
using SkiaSharp;

namespace BasicDrawingApp.Controls;

public static class DrawingRenderer
{
    public static void DrawDocument(ICanvas canvas, DrawingDocument document, DrawingShape? previewShape = null, RectF? viewport = null)
    {
        float width = viewport?.Width > 0 ? viewport.Value.Width : document.CanvasWidth;
        float height = viewport?.Height > 0 ? viewport.Value.Height : document.CanvasHeight;
        canvas.FillColor = Colors.White;
        canvas.FillRectangle(0, 0, width, height);

        foreach (DrawingShape shape in document.Shapes)
        {
            DrawShape(canvas, shape);
        }

        if (previewShape is not null)
        {
            canvas.Alpha = 0.65f;
            canvas.StrokeColor = Colors.DodgerBlue;
            DrawShape(canvas, previewShape);
            canvas.Alpha = 1f;
        }
    }

    public static void DrawShape(ICanvas canvas, DrawingShape shape)
    {
        canvas.StrokeColor = shape.StrokeColor;
        canvas.StrokeSize = Math.Max(1, shape.StrokeThickness);
        canvas.FillColor = shape.IsFilled ? shape.FillColor : Colors.Transparent;

        switch (shape.Kind)
        {
            case ShapeKind.Point:
                DrawPoint(canvas, shape);
                break;
            case ShapeKind.Line:
                canvas.DrawLine(shape.StartPoint, shape.EndPoint);
                break;
            case ShapeKind.Ellipse:
                DrawEllipse(canvas, NormalizeRect(shape.StartPoint, shape.EndPoint), shape.IsFilled);
                break;
            case ShapeKind.Circle:
                DrawEllipse(canvas, GetSquareRect(shape.StartPoint, shape.EndPoint), shape.IsFilled);
                break;
            case ShapeKind.Square:
                DrawRectangle(canvas, GetSquareRect(shape.StartPoint, shape.EndPoint), shape.IsFilled);
                break;
            case ShapeKind.Rectangle:
                DrawRectangle(canvas, NormalizeRect(shape.StartPoint, shape.EndPoint), shape.IsFilled);
                break;
        }
    }

    public static void DrawDocument(SKCanvas canvas, DrawingDocument document)
    {
        using SKPaint background = new()
        {
            Color = SKColors.White,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawRect(0, 0, document.CanvasWidth, document.CanvasHeight, background);

        foreach (DrawingShape shape in document.Shapes)
        {
            DrawShape(canvas, shape);
        }
    }

    public static void DrawShape(SKCanvas canvas, DrawingShape shape)
    {
        using SKPaint stroke = new()
        {
            Color = ToSkColor(shape.StrokeColor),
            StrokeWidth = Math.Max(1, shape.StrokeThickness),
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round
        };

        using SKPaint fill = new()
        {
            Color = ToSkColor(shape.FillColor),
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        switch (shape.Kind)
        {
            case ShapeKind.Point:
                float radius = Math.Max(3, shape.StrokeThickness * 1.8f);
                if (shape.IsFilled)
                {
                    canvas.DrawCircle(shape.StartPoint.X, shape.StartPoint.Y, radius, fill);
                }
                canvas.DrawCircle(shape.StartPoint.X, shape.StartPoint.Y, radius, stroke);
                break;
            case ShapeKind.Line:
                canvas.DrawLine(shape.StartPoint.X, shape.StartPoint.Y, shape.EndPoint.X, shape.EndPoint.Y, stroke);
                break;
            case ShapeKind.Ellipse:
                DrawSkOval(canvas, NormalizeRect(shape.StartPoint, shape.EndPoint), shape.IsFilled, fill, stroke);
                break;
            case ShapeKind.Circle:
                DrawSkOval(canvas, GetSquareRect(shape.StartPoint, shape.EndPoint), shape.IsFilled, fill, stroke);
                break;
            case ShapeKind.Square:
                DrawSkRect(canvas, GetSquareRect(shape.StartPoint, shape.EndPoint), shape.IsFilled, fill, stroke);
                break;
            case ShapeKind.Rectangle:
                DrawSkRect(canvas, NormalizeRect(shape.StartPoint, shape.EndPoint), shape.IsFilled, fill, stroke);
                break;
        }
    }

    public static RectF NormalizeRect(PointF start, PointF end)
    {
        float x = Math.Min(start.X, end.X);
        float y = Math.Min(start.Y, end.Y);
        float width = Math.Abs(end.X - start.X);
        float height = Math.Abs(end.Y - start.Y);
        return new RectF(x, y, width, height);
    }

    public static RectF GetSquareRect(PointF start, PointF end)
    {
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float side = Math.Min(Math.Abs(dx), Math.Abs(dy));
        float x = dx >= 0 ? start.X : start.X - side;
        float y = dy >= 0 ? start.Y : start.Y - side;
        return new RectF(x, y, side, side);
    }

    private static void DrawPoint(ICanvas canvas, DrawingShape shape)
    {
        float radius = Math.Max(3, shape.StrokeThickness * 1.8f);
        RectF bounds = new(shape.StartPoint.X - radius, shape.StartPoint.Y - radius, radius * 2, radius * 2);
        DrawEllipse(canvas, bounds, shape.IsFilled);
    }

    private static void DrawRectangle(ICanvas canvas, RectF rect, bool isFilled)
    {
        if (isFilled)
        {
            canvas.FillRectangle(rect);
        }
        canvas.DrawRectangle(rect);
    }

    private static void DrawEllipse(ICanvas canvas, RectF rect, bool isFilled)
    {
        if (isFilled)
        {
            canvas.FillEllipse(rect);
        }
        canvas.DrawEllipse(rect);
    }

    private static void DrawSkRect(SKCanvas canvas, RectF rect, bool isFilled, SKPaint fill, SKPaint stroke)
    {
        SKRect skRect = ToSkRect(rect);
        if (isFilled)
        {
            canvas.DrawRect(skRect, fill);
        }
        canvas.DrawRect(skRect, stroke);
    }

    private static void DrawSkOval(SKCanvas canvas, RectF rect, bool isFilled, SKPaint fill, SKPaint stroke)
    {
        SKRect skRect = ToSkRect(rect);
        if (isFilled)
        {
            canvas.DrawOval(skRect, fill);
        }
        canvas.DrawOval(skRect, stroke);
    }

    private static SKRect ToSkRect(RectF rect)
    {
        return new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
    }

    private static SKColor ToSkColor(Color color)
    {
        color.ToRgba(out byte r, out byte g, out byte b, out byte a);
        return new SKColor(r, g, b, a);
    }
}
