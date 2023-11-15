using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using Localizer.Core.Resx;
using Localizer.Events;
using Localizer.Helper;
using Localizer.Updater;
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
        Dispatcher.UIThread.Invoke(() =>
        {
            if (viewModel.IsResxContentLoaded)
            {
                EventBus.Instance.ClearAllSubscriptions();
                MainWindow window = new MainWindow(viewModel.SolutionFolder);
                window.Show();
                Close();
            }
        });

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

        NewVersionUpdateChecker.ExecuteUpdateOnClose();
    }
}