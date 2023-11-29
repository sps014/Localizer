using Avalonia.Controls;
using Avalonia.Interactivity;
using Localizer.ViewModels;

namespace Localizer.Views
{
    public partial class SettingsWindow : Window
    {
        SettingsWindowViewModel viewModel;
        public SettingsWindow()
        {
            InitializeComponent();
            viewModel = new SettingsWindowViewModel();
            DataContext = viewModel;

            MainWindow.SetThemeOfWindow(this);
        }
    }
}
