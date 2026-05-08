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
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        if (Width < 760)
        {
            RootGrid.ColumnDefinitions = [new ColumnDefinition(GridLength.Star)];
            RootGrid.RowDefinitions =
            [
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star)
            ];

            Grid.SetRow(ToolScroll, 1);
            Grid.SetColumn(ToolScroll, 0);
            Grid.SetRow(CanvasBorder, 2);
            Grid.SetColumn(CanvasBorder, 0);
            ToolScroll.MaximumHeightRequest = 330;
            return;
        }

        RootGrid.ColumnDefinitions =
        [
            new ColumnDefinition(new GridLength(320)),
            new ColumnDefinition(GridLength.Star)
        ];
        RootGrid.RowDefinitions =
        [
            new RowDefinition(GridLength.Auto),
            new RowDefinition(GridLength.Star)
        ];

        Grid.SetRow(ToolScroll, 1);
        Grid.SetColumn(ToolScroll, 0);
        Grid.SetRow(CanvasBorder, 1);
        Grid.SetColumn(CanvasBorder, 1);
        ToolScroll.MaximumHeightRequest = -1;
    }
}
