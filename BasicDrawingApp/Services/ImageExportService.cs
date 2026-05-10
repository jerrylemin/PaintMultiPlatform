using BasicDrawingApp.Controls;
using BasicDrawingApp.Models;
using SkiaSharp;

namespace BasicDrawingApp.Services;

public sealed class ImageExportService
{
    private readonly IImageGalleryService _imageGalleryService;

    public ImageExportService(IImageGalleryService imageGalleryService)
    {
        _imageGalleryService = imageGalleryService;
    }

    public Task<string> ExportPngAsync(DrawingDocument document)
    {
        return ExportAsync(document, SKEncodedImageFormat.Png, 100, "png");
    }

    public Task<string> ExportJpegAsync(DrawingDocument document)
    {
        return ExportAsync(document, SKEncodedImageFormat.Jpeg, 90, "jpg");
    }

    private async Task<string> ExportAsync(DrawingDocument document, SKEncodedImageFormat format, int quality, string extension)
    {
        int width = Math.Max(1, (int)MathF.Ceiling(document.CanvasWidth));
        int height = Math.Max(1, (int)MathF.Ceiling(document.CanvasHeight));
        using SKBitmap bitmap = new(width, height);
        using SKCanvas canvas = new(bitmap);
        DrawingRenderer.DrawDocument(canvas, document);
        canvas.Flush();

        using SKImage image = SKImage.FromBitmap(bitmap);
        using SKData data = image.Encode(format, quality);
        byte[] imageBytes = data.ToArray();
        string mimeType = format == SKEncodedImageFormat.Jpeg ? "image/jpeg" : "image/png";
        string fileName = $"drawing_{DateTime.Now:yyyyMMdd_HHmmss}.{extension.TrimStart('.')}";

        ImageSaveResult? result = await _imageGalleryService.SaveImageAsync(imageBytes, extension, mimeType, fileName);
        if (result is null)
        {
            throw new OperationCanceledException("Export canceled.");
        }

        return result.UserMessage;
    }
}
