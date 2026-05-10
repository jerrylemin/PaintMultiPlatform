using Android.Content;
using Android.OS;
using Android.Provider;
using BasicDrawingApp.Services;
using AndroidEnvironment = Android.OS.Environment;

namespace BasicDrawingApp.Platforms.Android;

public sealed class AndroidImageGalleryService : IImageGalleryService
{
    public async Task<ImageSaveResult?> SaveImageAsync(byte[] imageBytes, string extension, string mimeType, string suggestedFileName)
    {
        ContentResolver resolver = Platform.CurrentActivity?.ContentResolver
            ?? throw new InvalidOperationException("Android ContentResolver is not available.");

        string normalizedExtension = extension.TrimStart('.');
        string fileName = Path.ChangeExtension(suggestedFileName, normalizedExtension);

        using ContentValues values = new();
        values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
        values.Put(MediaStore.IMediaColumns.MimeType, mimeType);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
        {
#pragma warning disable CA1416
            values.Put(MediaStore.IMediaColumns.RelativePath, $"{AndroidEnvironment.DirectoryPictures}/BasicDrawingApp");
            values.Put(MediaStore.IMediaColumns.IsPending, 1);
#pragma warning restore CA1416
        }

        global::Android.Net.Uri imageCollection = MediaStore.Images.Media.ExternalContentUri
            ?? throw new IOException("Android MediaStore image collection is not available.");
        global::Android.Net.Uri? uri = resolver.Insert(imageCollection, values);
        if (uri is null)
        {
            throw new IOException("Could not create an Android MediaStore image entry.");
        }

        try
        {
            await using Stream output = resolver.OpenOutputStream(uri)
                ?? throw new IOException("Could not open Android MediaStore output stream.");

            await output.WriteAsync(imageBytes);
            await output.FlushAsync();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
#pragma warning disable CA1416
                using ContentValues publishValues = new();
                publishValues.Put(MediaStore.IMediaColumns.IsPending, 0);
                resolver.Update(uri, publishValues, null, null);
#pragma warning restore CA1416
            }

            return new ImageSaveResult(
                fileName,
                "Photos/Gallery > Pictures/BasicDrawingApp",
                $"Exported to Photos/Gallery: {fileName} in Pictures/BasicDrawingApp");
        }
        catch
        {
            resolver.Delete(uri, null, null);
            throw;
        }
    }
}
