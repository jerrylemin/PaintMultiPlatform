namespace BasicDrawingApp.Services;

public sealed class FilePickerService
{
    public string DrawingDirectory
    {
        get
        {
            string directory = Path.Combine(FileSystem.AppDataDirectory, "Drawings");
            Directory.CreateDirectory(directory);
            return directory;
        }
    }

    public string ExportDirectory
    {
        get
        {
#if WINDOWS
            string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string directory = Path.Combine(string.IsNullOrWhiteSpace(pictures) ? FileSystem.AppDataDirectory : pictures, "BasicDrawingApp");
#else
            string directory = Path.Combine(FileSystem.AppDataDirectory, "Exports");
#endif
            Directory.CreateDirectory(directory);
            return directory;
        }
    }

    public Task<string> CreateSavePathAsync()
    {
        string fileName = $"drawing_{DateTime.Now:yyyyMMdd_HHmmss}.bdraw";
        return Task.FromResult(Path.Combine(DrawingDirectory, fileName));
    }

    public async Task<string?> PickBdrawFileAsync()
    {
        FilePickerFileType fileType = new(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, [".bdraw"] },
            { DevicePlatform.Android, ["application/octet-stream", "*/*"] }
        });

        PickOptions options = new()
        {
            PickerTitle = "Open .bdraw drawing",
            FileTypes = fileType
        };

        FileResult? result = await FilePicker.Default.PickAsync(options);
        return result?.FullPath;
    }

    public string CreateExportPath(string extension)
    {
        string fileName = $"drawing_{DateTime.Now:yyyyMMdd_HHmmss}.{extension.TrimStart('.')}";
        return Path.Combine(ExportDirectory, fileName);
    }
}
