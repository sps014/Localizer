using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Localizer.ViewModels;

namespace Localizer;

public partial class FindReplaceWindow : Window
{
    FindReplaceViewModel viewModel;
    public FindReplaceWindow()
    {
        InitializeComponent();
        viewModel = new FindReplaceViewModel();
        MainWindow.SetThemeOfWindow(this);
        DataContext = viewModel;
        findBox.Focus();
    }

    private void closeBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }

    private void findBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        viewModel.ResetIndex();
    }
}