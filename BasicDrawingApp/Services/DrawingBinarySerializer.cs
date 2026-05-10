using BasicDrawingApp.Models;

namespace BasicDrawingApp.Services;

public sealed class DrawingBinarySerializer
{
    private const string Magic = "BDRAW";
    private const int Version = 1;

    public async Task SaveAsync(DrawingDocument document, string filePath)
    {
        await using FileStream stream = File.Create(filePath);
        await SaveAsync(document, stream);
    }

    public async Task SaveAsync(DrawingDocument document, Stream stream)
    {
        using BinaryWriter writer = new(stream);

        writer.Write(Magic);
        writer.Write(Version);
        writer.Write(document.CanvasWidth);
        writer.Write(document.CanvasHeight);
        writer.Write(document.Shapes.Count);

        foreach (DrawingShape shape in document.Shapes)
        {
            writer.Write(shape.Id);
            writer.Write((int)shape.Kind);
            writer.Write(shape.StartPoint.X);
            writer.Write(shape.StartPoint.Y);
            writer.Write(shape.EndPoint.X);
            writer.Write(shape.EndPoint.Y);
            WriteColor(writer, shape.StrokeColor);
            WriteColor(writer, shape.FillColor);
            writer.Write(shape.StrokeThickness);
            writer.Write(shape.IsFilled);
            writer.Write(shape.CreatedAt.Ticks);
        }

        await stream.FlushAsync();
    }

    public async Task<DrawingDocument> LoadAsync(string filePath)
    {
        await using FileStream stream = File.OpenRead(filePath);
        return await LoadAsync(stream);
    }

    public Task<DrawingDocument> LoadAsync(Stream stream)
    {
        using BinaryReader reader = new(stream);

        try
        {
            string magic = reader.ReadString();
            if (!string.Equals(magic, Magic, StringComparison.Ordinal))
            {
                throw new InvalidDataException("File is not a BDRAW drawing.");
            }

            int version = reader.ReadInt32();
            if (version != Version)
            {
                throw new InvalidDataException($"Unsupported BDRAW version: {version}.");
            }

            DrawingDocument document = new()
            {
                CanvasWidth = reader.ReadSingle(),
                CanvasHeight = reader.ReadSingle(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            int shapeCount = reader.ReadInt32();
            if (shapeCount < 0)
            {
                throw new InvalidDataException("Shape count is invalid.");
            }

            for (int i = 0; i < shapeCount; i++)
            {
                document.Shapes.Add(new DrawingShape
                {
                    Id = reader.ReadString(),
                    Kind = (ShapeKind)reader.ReadInt32(),
                    StartPoint = new PointF(reader.ReadSingle(), reader.ReadSingle()),
                    EndPoint = new PointF(reader.ReadSingle(), reader.ReadSingle()),
                    StrokeColor = ReadColor(reader),
                    FillColor = ReadColor(reader),
                    StrokeThickness = reader.ReadSingle(),
                    IsFilled = reader.ReadBoolean(),
                    CreatedAt = new DateTime(reader.ReadInt64(), DateTimeKind.Utc)
                });
            }

            return Task.FromResult(document);
        }
        catch (EndOfStreamException ex)
        {
            throw new InvalidDataException("The BDRAW file is incomplete or corrupted.", ex);
        }
    }

    private static void WriteColor(BinaryWriter writer, Color color)
    {
        color.ToRgba(out byte r, out byte g, out byte b, out byte a);
        writer.Write(r);
        writer.Write(g);
        writer.Write(b);
        writer.Write(a);
    }

    private static Color ReadColor(BinaryReader reader)
    {
        byte r = reader.ReadByte();
        byte g = reader.ReadByte();
        byte b = reader.ReadByte();
        byte a = reader.ReadByte();
        return Color.FromRgba(r, g, b, a);
    }
}
