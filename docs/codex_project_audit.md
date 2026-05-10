# Codex Project Audit

## Repository Scan

- Read `README.md`.
- Read all files under `docs/`.
- Listed the whole repository, including generated `bin/` and `obj/` files.
- Read source files outside generated folders, including `MainPage.xaml`, `MainPage.xaml.cs`, `MainViewModel.cs`, `DrawingBinarySerializer.cs`, `ImageExportService.cs`, save/load/export services, `Platforms/Android/*`, `Platforms/Windows/*`, `AndroidManifest.xml`, and `BasicDrawingApp.csproj`.
- Generated folders were not edited.

## Stack And Entrypoints

- App type: .NET MAUI single-project app.
- Targets: `net9.0-android` and `net9.0-windows10.0.19041.0` on Windows.
- Package manager: NuGet through `BasicDrawingApp.csproj`.
- Entrypoint registration: `BasicDrawingApp/MauiProgram.cs`.
- Main UI: `BasicDrawingApp/Views/MainPage.xaml`.
- Main state: `BasicDrawingApp/ViewModels/MainViewModel.cs`.

## Android Save `.bdraw` Finding

Before this fix, Android `Save .bdraw` wrote a file to:

`FileSystem.AppDataDirectory/Drawings/drawing_yyyyMMdd_HHmmss.bdraw`

On Android this resolves to an app-private folder, typically:

`/data/user/0/com.companyname.basicdrawingapp/files/Drawings`

That folder is valid for the app, but normal Android file picker and Photos/Gallery do not show it. The app then used FilePicker for load, so the user could not reliably select the private saved `.bdraw` file again. This made save look broken even when bytes were written.

## Android Export Finding

Before this fix, image export first wrote to `FileSystem.AppDataDirectory/Exports` and then attempted a MediaStore publish from that private file. The status was not clear enough for Android users, and the app still depended on a shared service that mixed private app storage with gallery publishing. If MediaStore insert or output stream failed, the fallback path was a private app path that Photos/Gallery cannot show.

Photos/Gallery indexes MediaStore image entries, not arbitrary app-private files. PNG/JPEG export must write to `MediaStore.Images.Media.ExternalContentUri` with a correct display name and MIME type.

## Load `.bdraw` Finding

The binary loader already checked the `BDRAW` magic header, version, shape count, and shape records. The main Android problem was the source of the file, not the serializer format.

Android FilePicker is useful for importing external `.bdraw` files, but it is not the right UI for app-private saved drawings. The fixed Android load flow now supports both:

- `Load from App Files`: lists `.bdraw` files saved under the app documents folder.
- `Import .bdraw`: opens Android FilePicker and reads the selected file stream.

## Files Fixed

- `BasicDrawingApp/MauiProgram.cs`: registers platform-specific file/gallery services.
- `BasicDrawingApp/Services/FileServiceModels.cs`: adds shared service contracts and result records.
- `BasicDrawingApp/Services/DrawingBinarySerializer.cs`: adds stream save/load support for Android FilePicker/content streams.
- `BasicDrawingApp/Services/ImageExportService.cs`: renders PNG/JPEG bytes and delegates platform persistence.
- `BasicDrawingApp/ViewModels/MainViewModel.cs`: uses platform services, clearer status messages, and debug logging on failures.
- `BasicDrawingApp/Views/MainPage.xaml`: binds save button text per platform.
- `BasicDrawingApp/Platforms/Android/AndroidDrawingFileService.cs`: saves `.bdraw` to app files and loads from internal list or import picker.
- `BasicDrawingApp/Platforms/Android/AndroidImageGalleryService.cs`: writes PNG/JPEG directly to MediaStore.
- `BasicDrawingApp/Platforms/Windows/WindowsDrawingFileService.cs`: uses Windows save/open picker for `.bdraw`.
- `BasicDrawingApp/Platforms/Windows/WindowsImageSaveService.cs`: uses Windows save picker for PNG/JPEG.
- `README.md`, `docs/codex_progress.md`, and `docs/codex_test_report.md`: updated Android behavior and verification notes.

## Current Paths

- Android `.bdraw`: `FileSystem.AppDataDirectory/BasicDrawingApp/drawing_yyyyMMdd_HHmmss.bdraw`.
- Android gallery export: `Pictures/BasicDrawingApp/drawing_yyyyMMdd_HHmmss.png` or `.jpg` through MediaStore.
- Windows `.bdraw`: user-selected Save Picker path.
- Windows PNG/JPEG: user-selected Save Picker path.

## Permissions

No broad storage permission was added. Android 10+ MediaStore writes do not require `MANAGE_EXTERNAL_STORAGE`, and the app does not request it.
