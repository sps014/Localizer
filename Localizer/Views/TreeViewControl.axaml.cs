using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using Localizer.Core.Model;
using Localizer.Core.Resx;
using Localizer.ViewModels;

namespace Localizer;

public partial class TreeViewControl : UserControl
{
   private TreeControlViewModel viewModel;
    public TreeViewControl()
    {
        
        InitializeComponent();
        viewModel = new TreeControlViewModel();
        DataContext = viewModel;
    }
    private void AutoCompleteBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        viewModel.SearchNodes((sender as AutoCompleteBox)!.Text!);
    }
}