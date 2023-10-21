using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;

namespace Localizer.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        if (IsWindows11)
        {
            TransparencyLevelHint = new[] { WindowTransparencyLevel.Mica };
        }
        else
        {
            TransparencyLevelHint = new[] { WindowTransparencyLevel.None };
            Background = new SolidColorBrush(Colors.Black);
        }
    }

    bool IsWindows11
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
}