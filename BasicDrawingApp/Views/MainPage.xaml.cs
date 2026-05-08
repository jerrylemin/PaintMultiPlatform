using BasicDrawingApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BasicDrawingApp.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
        : this(IPlatformApplication.Current?.Services.GetRequiredService<MainViewModel>()
            ?? throw new InvalidOperationException("Application services are not available."))
    {
    }

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
