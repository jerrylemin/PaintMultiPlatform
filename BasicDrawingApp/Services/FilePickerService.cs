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

    public async Task<string?> PickSaveBdrawPathAsync()
    {
        string fileName = $"drawing_{DateTime.Now:yyyyMMdd_HHmmss}.bdraw";
#if WINDOWS
        return await PickWindowsSavePathAsync(fileName, ".bdraw", "BDRAW drawing");
#else
        return Path.Combine(DrawingDirectory, fileName);
#endif
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

    public async Task<string?> PickExportPathAsync(string extension)
    {
        string fileName = $"drawing_{DateTime.Now:yyyyMMdd_HHmmss}.{extension.TrimStart('.')}";
#if WINDOWS
        string normalizedExtension = $".{extension.TrimStart('.')}";
        string label = normalizedExtension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
            ? "JPEG image"
            : "PNG image";
        return await PickWindowsSavePathAsync(fileName, normalizedExtension, label);
#else
        return Path.Combine(ExportDirectory, fileName);
#endif
    }

    public async Task<string> PublishImageAsync(string sourceFilePath, string extension)
    {
#if ANDROID
        string displayName = Path.GetFileName(sourceFilePath);
        string mimeType = extension.Equals("jpg", StringComparison.OrdinalIgnoreCase) || extension.Equals("jpeg", StringComparison.OrdinalIgnoreCase)
            ? "image/jpeg"
            : "image/png";

        Android.Content.ContentResolver? resolver = Platform.CurrentActivity?.ContentResolver;
        if (resolver is null)
        {
            return sourceFilePath;
        }

        using Android.Content.ContentValues values = new();
        values.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, displayName);
        values.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, mimeType);
        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Q)
        {
#pragma warning disable CA1416
            values.Put(Android.Provider.MediaStore.IMediaColumns.RelativePath, Android.OS.Environment.DirectoryPictures + "/BasicDrawingApp");
#pragma warning restore CA1416
        }

        Android.Net.Uri? imageCollection = Android.Provider.MediaStore.Images.Media.ExternalContentUri;
        if (imageCollection is null)
        {
            return sourceFilePath;
        }

        Android.Net.Uri? uri = resolver.Insert(imageCollection, values);
        if (uri is null)
        {
            return sourceFilePath;
        }

        await using Stream? output = resolver.OpenOutputStream(uri);
        await using FileStream input = File.OpenRead(sourceFilePath);
        if (output is not null)
        {
            await input.CopyToAsync(output);
            await output.FlushAsync();
            return uri.ToString() ?? sourceFilePath;
        }
#endif
        return sourceFilePath;
    }

#if WINDOWS
    private static async Task<string?> PickWindowsSavePathAsync(string suggestedFileName, string extension, string label)
    {
        Windows.Storage.Pickers.FileSavePicker picker = new()
        {
            SuggestedFileName = Path.GetFileNameWithoutExtension(suggestedFileName)
        };
        picker.FileTypeChoices.Add(label, [extension]);

        Microsoft.UI.Xaml.Window? nativeWindow = Application.Current?.Windows.FirstOrDefault()?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        if (nativeWindow is not null)
        {
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        }

        Windows.Storage.StorageFile? file = await picker.PickSaveFileAsync();
        return file?.Path;
    }
#endif
}
