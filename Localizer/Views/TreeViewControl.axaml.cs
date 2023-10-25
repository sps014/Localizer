using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using Localizer.Core.Model;
using Localizer.Core.Resx;
using Localizer.Events;
using Localizer.ViewModels;

namespace Localizer;

public partial class TreeViewControl : UserControl
{
   private TreeControlViewModel viewModel;
    public TreeViewControl()
    {

        InitializeComponent();
        viewModel = new TreeControlViewModel()
        {
            TreeView = treeNode
        };
        DataContext = viewModel;
    }
    private void AutoCompleteBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        viewModel.SearchNodes();
    }

    private void TreeView_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        var dt = treeNode.SelectedItem as ResxFileSystemNodeBase;
        if(dt == null)
            return;

        viewModel.OnTreeNodeSelection(dt);
    }
    private void Menu_Tapped(object? sender, TappedEventArgs e)
    {
        var dt = (sender as Menu);
        if (dt == null)
            return;
        var dtc = dt.DataContext as ResxFileSystemNodeBase;

        if (dtc==null) 
            return;

        EventBus.Instance.Publish(new TreeRequestsLoadDisplayEntryEvent(dtc));
    }
}