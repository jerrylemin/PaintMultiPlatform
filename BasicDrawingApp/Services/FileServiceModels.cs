using BasicDrawingApp.Models;

namespace BasicDrawingApp.Services;

public sealed record DrawingFileSaveResult(string FileName, string Path, string UserMessage);

public sealed record DrawingFileLoadResult(DrawingDocument Document, string FileName, string Path, string UserMessage);

public sealed record DrawingFileInfo(string FileName, string Path, long SizeBytes, DateTime LastModified);

public sealed record ImageSaveResult(string FileName, string Location, string UserMessage);

public interface IDrawingFileService
{
    string SaveButtonText { get; }

    Task<DrawingFileSaveResult?> SaveAsync(DrawingDocument document, DrawingBinarySerializer serializer);

    Task<DrawingFileLoadResult?> LoadAsync(DrawingBinarySerializer serializer);
}

public interface IImageGalleryService
{
    Task<ImageSaveResult?> SaveImageAsync(byte[] imageBytes, string extension, string mimeType, string suggestedFileName);
}
