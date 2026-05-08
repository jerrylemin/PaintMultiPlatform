using BasicDrawingApp.Controls;
using BasicDrawingApp.Models;
using SkiaSharp;

namespace BasicDrawingApp.Services;

public sealed class ImageExportService
{
    private readonly FilePickerService _filePickerService;

    public ImageExportService(FilePickerService filePickerService)
    {
        _filePickerService = filePickerService;
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
        string filePath = _filePickerService.CreateExportPath(extension);

        using SKBitmap bitmap = new(width, height);
        using SKCanvas canvas = new(bitmap);
        DrawingRenderer.DrawDocument(canvas, document);
        canvas.Flush();

        using SKImage image = SKImage.FromBitmap(bitmap);
        using SKData data = image.Encode(format, quality);
        await using FileStream stream = File.Create(filePath);
        data.SaveTo(stream);
        await stream.FlushAsync();

#if ANDROID
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share exported drawing",
            File = new ShareFile(filePath)
        });
#endif

        return filePath;
    }
}
