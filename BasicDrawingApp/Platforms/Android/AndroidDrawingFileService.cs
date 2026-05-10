using BasicDrawingApp.Models;
using BasicDrawingApp.Services;

namespace BasicDrawingApp.Platforms.Android;

public sealed class AndroidDrawingFileService : IDrawingFileService
{
    private const string AppFolderName = "BasicDrawingApp";

    public string SaveButtonText => "Save .bdraw to App Files";

    private static string DrawingDirectory
    {
        get
        {
            string directory = Path.Combine(FileSystem.AppDataDirectory, AppFolderName);
            Directory.CreateDirectory(directory);
            return directory;
        }
    }

    public async Task<DrawingFileSaveResult?> SaveAsync(DrawingDocument document, DrawingBinarySerializer serializer)
    {
        string fileName = $"drawing_{DateTime.Now:yyyyMMdd_HHmmss}.bdraw";
        string path = Path.Combine(DrawingDirectory, fileName);
        await serializer.SaveAsync(document, path);

        FileInfo file = new(path);
        return new DrawingFileSaveResult(
            fileName,
            path,
            $"Saved to app documents: {fileName} ({file.Length} bytes)");
    }

    public async Task<DrawingFileLoadResult?> LoadAsync(DrawingBinarySerializer serializer)
    {
        string action = await Shell.Current.DisplayActionSheet(
            "Load .bdraw",
            "Cancel",
            null,
            "Load from App Files",
            "Import .bdraw");

        return action switch
        {
            "Load from App Files" => await LoadFromAppFilesAsync(serializer),
            "Import .bdraw" => await ImportAsync(serializer),
            _ => null
        };
    }

    private static async Task<DrawingFileLoadResult?> LoadFromAppFilesAsync(DrawingBinarySerializer serializer)
    {
        DrawingFileInfo[] files = Directory
            .EnumerateFiles(DrawingDirectory, "*.bdraw")
            .Select(path =>
            {
                FileInfo info = new(path);
                return new DrawingFileInfo(info.Name, info.FullName, info.Length, info.LastWriteTime);
            })
            .OrderByDescending(file => file.LastModified)
            .ToArray();

        if (files.Length == 0)
        {
            await Shell.Current.DisplayAlert("No drawings", "No .bdraw files were found in app documents.", "OK");
            return null;
        }

        string selectedName = await Shell.Current.DisplayActionSheet(
            "Load from App Files",
            "Cancel",
            null,
            files.Select(file => file.FileName).ToArray());

        DrawingFileInfo? selected = files.FirstOrDefault(file => string.Equals(file.FileName, selectedName, StringComparison.Ordinal));
        if (selected is null)
        {
            return null;
        }

        DrawingDocument document = await serializer.LoadAsync(selected.Path);
        return new DrawingFileLoadResult(
            document,
            selected.FileName,
            selected.Path,
            $"Loaded {document.Shapes.Count} shapes from app files: {selected.FileName}");
    }

    private static async Task<DrawingFileLoadResult?> ImportAsync(DrawingBinarySerializer serializer)
    {
        FilePickerFileType fileType = new(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.Android, ["application/octet-stream", "application/x-bdraw", "*/*"] }
        });

        PickOptions options = new()
        {
            PickerTitle = "Import .bdraw drawing",
            FileTypes = fileType
        };

        FileResult? result = await FilePicker.Default.PickAsync(options);
        if (result is null)
        {
            return null;
        }

        await using Stream stream = await result.OpenReadAsync();
        DrawingDocument document = await serializer.LoadAsync(stream);
        return new DrawingFileLoadResult(
            document,
            result.FileName,
            result.FullPath ?? result.FileName,
            $"Loaded {document.Shapes.Count} shapes from imported file: {result.FileName}");
    }
}
