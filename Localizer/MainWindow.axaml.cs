using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
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
    }

    public static void SetThemeOfWindow(Window window)
    {
        if (IsWindows11)
        {
            window.TransparencyLevelHint = new[] { WindowTransparencyLevel.Mica };
        }
        else
        {
            window.TransparencyLevelHint = new[] { WindowTransparencyLevel.None };
            window.Background = new SolidColorBrush(Colors.Black);
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
}