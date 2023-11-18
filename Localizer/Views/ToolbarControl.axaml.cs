using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Localizer.Events;
using Localizer.Helper;
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

    private void refresh_Click(object sender, RoutedEventArgs e)
    {
        EventBus.Instance.Publish(new ReloadResourcesEvent());

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
    async void importExlClick(object sender, RoutedEventArgs e)
    {
        var open = await TopLevel.GetTopLevel(this)!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import to Excel",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("Excel File")
                {
                    Patterns = new[]{"*.xlsx"},
                    MimeTypes = new[]{"xlsx/*"},
                },
            }
        });

        if(open==null || open.Count<=0 || open.First().Path==null || open.First().Path.LocalPath==null)
        {
            return;
        }

        var path = open.First().Path.LocalPath;
        EventBus.Instance.Publish(new ImportFromExcelEvent(path,MainWindowViewModel.Instance.ResxManager));

    }

    async void exportExlClick(object sender, RoutedEventArgs e)
    {
        var save = await TopLevel.GetTopLevel(this)!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export to Excel",
            DefaultExtension = "xlsx",
            ShowOverwritePrompt = true,
            FileTypeChoices = new List<FilePickerFileType>
            {
                new("Excel File")
                {
                    Patterns = new[]{"*.xlsx"},
                    MimeTypes = new[]{"xlsx/*"},
                },
            }

        });
        if(save==null || save.Path==null) return;

        var fileName = save.Path.LocalPath;

        EventBus.Instance.Publish(new ExportToExcelEvent(fileName));
    }
    async void exportSnapClick(object sender, RoutedEventArgs e)
    {
        var save = await TopLevel.GetTopLevel(this)!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Create Snapshot",
            DefaultExtension = "json",
            ShowOverwritePrompt = true,
            FileTypeChoices = new List<FilePickerFileType>
            {
                new("Json File")
                {
                    Patterns = new[]{"*.json"},
                    MimeTypes = new[]{"json/*"},
                },
            }

        });
        if (save == null || save.Path == null) return;

        var fileName = save.Path.LocalPath;

        EventBus.Instance.Publish(new CreateSnapshotEvent(fileName));
    }
    async void importSnapClick(object sender, RoutedEventArgs e)
    {
        var open = await TopLevel.GetTopLevel(this)!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import Snapshot",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("Json File")
                {
                    Patterns = new[]{"*.json"},
                    MimeTypes = new[]{"json/*"},
                },
            }
        });

        if (open == null || open.Count <= 0 || open.First().Path == null || open.First().Path.LocalPath == null)
        {
            return;
        }

        var path = open.First().Path.LocalPath;
        EventBus.Instance.Publish(new LoadSnapshotEvent(path));

    }
    void unloadSnapClick(object sender, RoutedEventArgs e)
    {
        EventBus.Instance.Publish(new UnloadSnapshotEvent());
    }



    private void search_Click(object? sender, RoutedEventArgs e)
    {
        FindReplaceWindow fnd = new FindReplaceWindow();
        fnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        fnd.ShowDialog(WindowHelper.ParentWindow<MainWindow>()!);
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