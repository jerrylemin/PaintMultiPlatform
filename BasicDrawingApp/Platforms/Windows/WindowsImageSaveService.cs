using BasicDrawingApp.Services;
using Windows.Storage;

namespace BasicDrawingApp.Platforms.Windows;

public sealed class WindowsImageSaveService : IImageGalleryService
{
    public async Task<ImageSaveResult?> SaveImageAsync(byte[] imageBytes, string extension, string mimeType, string suggestedFileName)
    {
        string normalizedExtension = $".{extension.TrimStart('.')}";
        string label = mimeType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase)
            ? "JPEG image"
            : "PNG image";

        StorageFile? file = await WindowsDrawingFileService.PickSaveFileAsync(
            Path.GetFileNameWithoutExtension(suggestedFileName),
            normalizedExtension,
            label);

        if (file is null)
        {
            return null;
        }

        await File.WriteAllBytesAsync(file.Path, imageBytes);
        return new ImageSaveResult(file.Name, file.Path, $"Exported to: {file.Path}");
    }
}
