using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Localizer;

public partial class StartupPageWindow : Window
{
    public StartupPageWindow()
    {
        InitializeComponent();
        TransparencyLevelHint = new[] {WindowTransparencyLevel.Mica}; 
    }
}