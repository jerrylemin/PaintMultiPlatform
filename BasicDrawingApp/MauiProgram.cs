using BasicDrawingApp.Services;
using BasicDrawingApp.ViewModels;
using BasicDrawingApp.Views;
using Microsoft.Extensions.Logging;

namespace BasicDrawingApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<DrawingBinarySerializer>();
        builder.Services.AddSingleton<FilePickerService>();
        builder.Services.AddSingleton<ImageExportService>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
