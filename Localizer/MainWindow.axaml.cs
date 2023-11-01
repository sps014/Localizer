using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using Localizer.Events;
using Localizer.Helper;
using Localizer.ViewModels;

namespace Localizer;

public partial class MainWindow : Window
{
    MainWindowViewModel viewModel;
    public MainWindow(string selectedFolder)
    {
        viewModel = new MainWindowViewModel(selectedFolder);

        InitializeComponent();
        SetThemeOfWindow(this);
        DataContext = viewModel;

        EventBus.Instance.Subscribe<ReloadResourcesEvent>(ReloadResources);
    }

    public static void SetThemeOfWindow(Window window)
    {
        if (IsWindows11)
        {
            window.TransparencyLevelHint = new[] { WindowTransparencyLevel.Mica };
        }
        else
        {
            window.TransparencyLevelHint = new[] { WindowTransparencyLevel.AcrylicBlur };
            window.Background = new SolidColorBrush(Colors.Transparent);
        }

    }
    public static bool IsWindows11
    {
        get
        {
            if (OperatingSystem.IsWindows())
            {
                var version = Environment.OSVersion;
                return version.Version.Build >= 22000;
            }
            return false;
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        viewModel.LoadAsync();
    }

    private void ReloadResources(ReloadResourcesEvent e)
    {
        if (viewModel.IsResxContentLoaded)
        {
            viewModel = new MainWindowViewModel(viewModel.SolutionFolder);
            DataContext = viewModel;
            viewModel.LoadAsync();
        }
    }


    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        viewModel.RequestCacellationOfLoadingResx();
    }
    protected override void OnClosed(EventArgs e)
    {
        //save resx files info in csproj
        IncludeAllResxFiles.Build(viewModel.SolutionFolder);
    }
}