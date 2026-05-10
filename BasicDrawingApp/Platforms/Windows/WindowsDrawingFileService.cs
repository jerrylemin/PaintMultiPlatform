using BasicDrawingApp.Models;
using BasicDrawingApp.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace BasicDrawingApp.Platforms.Windows;

public sealed class WindowsDrawingFileService : IDrawingFileService
{
    public string SaveButtonText => "Save .bdraw";

    public async Task<DrawingFileSaveResult?> SaveAsync(DrawingDocument document, DrawingBinarySerializer serializer)
    {
        StorageFile? file = await PickSaveFileAsync("drawing", ".bdraw", "BDRAW drawing");
        if (file is null)
        {
            return null;
        }

        await serializer.SaveAsync(document, file.Path);
        return new DrawingFileSaveResult(file.Name, file.Path, $"Saved to: {file.Path}");
    }

    public async Task<DrawingFileLoadResult?> LoadAsync(DrawingBinarySerializer serializer)
    {
        FilePickerFileType fileType = new(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, [".bdraw"] }
        });

        PickOptions options = new()
        {
            PickerTitle = "Open .bdraw drawing",
            FileTypes = fileType
        };

        FileResult? result = await FilePicker.Default.PickAsync(options);
        if (result is null)
        {
            return null;
        }

        DrawingDocument document = await serializer.LoadAsync(result.FullPath);
        return new DrawingFileLoadResult(document, result.FileName, result.FullPath, $"Loaded {document.Shapes.Count} shapes from: {result.FullPath}");
    }

    internal static async Task<StorageFile?> PickSaveFileAsync(string suggestedName, string extension, string label)
    {
        FileSavePicker picker = new()
        {
            SuggestedFileName = $"{suggestedName}_{DateTime.Now:yyyyMMdd_HHmmss}"
        };
        picker.FileTypeChoices.Add(label, [extension]);

        Microsoft.UI.Xaml.Window? nativeWindow = Application.Current?.Windows.FirstOrDefault()?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        if (nativeWindow is not null)
        {
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        }

        return await picker.PickSaveFileAsync();
    }
}
