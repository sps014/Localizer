using System;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;

namespace Localizer;

public partial class App : Application
{
    public const string APP_NAME = "Localizer";
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        //set path of Environment Variable 
        Environment.SetEnvironmentVariable(APP_NAME, Process.GetCurrentProcess().MainModule.FileName,EnvironmentVariableTarget.User);

        var environmentArgs = Environment.GetCommandLineArgs();
        Window window = null;

        //if path is passed in command line
        if (environmentArgs != null && environmentArgs.Length > 1 && Directory.Exists(environmentArgs[1]))
        {
            window = new MainWindow(environmentArgs[1]);
        }
        else
            window = new StartupPageWindow();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}