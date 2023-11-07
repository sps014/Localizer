using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Localizer.ViewModels;

namespace Localizer;

public partial class StatusBarControl : UserControl
{
    public StatusBarControl()
    {
        InitializeComponent();
        DataContext = new StatusBarControlViewModel();
    }
}