using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Localizer.Events;
using Localizer.ViewModels;

namespace Localizer;

public partial class ToolbarControl : UserControl
{
    public ToolbarControl()
    {
        InitializeComponent();
        DataContext = this;
    }
    private ObservableCollection<TableColumnInfo>? toolbarColumns = null;
    internal ObservableCollection<TableColumnInfo> ToolbarColumns
    {
        get
        {
            if(MainWindowViewModel.Instance==null)
                return new ObservableCollection<TableColumnInfo>();

            if (toolbarColumns != null)
                return toolbarColumns;

            toolbarColumns = new ObservableCollection<TableColumnInfo>();
            foreach(var language in MainWindowViewModel.Instance.ResxManager.Tree.Cultures.OrderBy(x=>x))
            {
                toolbarColumns.Add(new TableColumnInfo(language,false,true));
                toolbarColumns.Add(new TableColumnInfo(language, true, true));
            }
            return toolbarColumns;
        }
    }

    private void langColmnCheckBox_Tapped(object sender,TappedEventArgs e)
    {
        PublishLanguageChanged(sender);
    }

    void cross_Clicked(object sender, RoutedEventArgs e)
    {
        EventBus.Instance.Publish(new RemoveKeyFromResourceEvent(true));
    }

    void PublishLanguageChanged(object sender)
    {
        var dt = ((CheckBox)sender).DataContext as TableColumnInfo;

        if (dt == null) return;

        EventBus.Instance.Publish(new TableColmnVisibilityChangeEvent(dt));
    }

    private void CheckBox_DoubleTapped(object? sender, TappedEventArgs e)
    {
        PublishLanguageChanged(sender);
    }

    private void search_Click(object? sender, RoutedEventArgs e)
    {
        FindReplaceWindow fnd = new FindReplaceWindow();
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            fnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            fnd.ShowDialog(desktop.Windows.First(x=>x is MainWindow));
        }
    }
}

internal record TableColumnInfo
{
    public bool IsVisible { get; set; }
    public string Culture { get; set; }
    public bool IsComment { get; set; }

    public string DisplayValue
    {
        get
        {
            var c = Culture==string.Empty ? "Neutral" : Culture;
            if (IsComment)
            {
                return $"Comment ({c})";
            }

            return c;
        }
    }

    public TableColumnInfo(string culture,bool isComment,bool isVisible)
    {
        Culture = culture;
        IsComment = isComment;
        IsVisible = isVisible;
    }
}