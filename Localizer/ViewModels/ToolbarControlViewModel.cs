using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Localizer.Events;
using Localizer.Helper;
using Localizer.Views;
using MsBox.Avalonia;

namespace Localizer.ViewModels;

public partial class ToolbarControlViewModel : ObservableObject
{

    [ObservableProperty]
    private bool isResxEntitySelected = false;

    private ObservableCollection<TableColumnInfo>? toolbarColumns = null;

    internal ObservableCollection<TableColumnInfo> ToolbarColumns
    {
        get
        {
            if (MainWindowViewModel.Instance == null)
                return new ObservableCollection<TableColumnInfo>();

            if (toolbarColumns != null)
                return toolbarColumns;

            toolbarColumns = new ObservableCollection<TableColumnInfo>();
            foreach (var language in MainWindowViewModel.Instance.ResxManager.Tree.Cultures.OrderBy(x => x))
            {
                toolbarColumns.Add(new TableColumnInfo(language, false, true));
                toolbarColumns.Add(new TableColumnInfo(language, true, true));
            }
            return toolbarColumns;
        }
    }

    public ToolbarControlViewModel()
    {
        EventBus.Instance.Subscribe<DataGridCurrentSelectionChangedevent>(SelectionOfDataGridItemChanged);
    }

    private void SelectionOfDataGridItemChanged(DataGridCurrentSelectionChangedevent e)
    {
        IsResxEntitySelected = e.Item is not null;
    }

    public void PublishLanguageChanged(object sender)
    {
        var dt = ((CheckBox)sender).DataContext as TableColumnInfo;

        if (dt == null) return;

        EventBus.Instance.Publish(new TableColmnVisibilityChangeEvent(dt));
    }

    [RelayCommand]
    public void Refresh()
    {
        EventBus.Instance.Publish(new ReloadResourcesEvent());
    }

    [RelayCommand]
    public void Delete()
    {
        EventBus.Instance.Publish(new RemoveKeyFromResourceEvent(true));
    }

    [RelayCommand]
    public async Task ImportExcel()
    {
        var open = await TopLevel
            .GetTopLevel(WindowHelper.ParentWindow<MainWindow>())!
            .StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
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

        if (open == null || open.Count <= 0 || open.First().Path == null || open.First().Path.LocalPath == null)
        {
            return;
        }

        var path = open.First().Path.LocalPath;
        EventBus.Instance.Publish(new ImportFromExcelEvent(path,
            MainWindowViewModel.Instance!.ResxManager));

    }

    [RelayCommand]
    public async Task ExportExcel()
    {
        var save = await TopLevel
            .GetTopLevel(WindowHelper.ParentWindow<MainWindow>())!
            .StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
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
        if (save == null || save.Path == null) return;

        var fileName = save.Path.LocalPath;

        EventBus.Instance.Publish(new ExportToExcelEvent(fileName));
    }

    [RelayCommand]
    public async Task ExportSnapshot()
    {
        var save = await TopLevel
            .GetTopLevel(WindowHelper.ParentWindow<MainWindow>())!.StorageProvider
            .SaveFilePickerAsync(new FilePickerSaveOptions
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

    [RelayCommand]
    public async Task ImportSnapshot()
    {
        var open = await TopLevel
            .GetTopLevel(WindowHelper.ParentWindow<MainWindow>())!
            .StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
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

    [RelayCommand]
    public void UnloadSnapshot()
    {
        EventBus.Instance.Publish(new UnloadSnapshotEvent());
    }

    [RelayCommand]
    public async Task ProcessDiff()
    {
        if (!SnapshotLoader.IsSnapshotLoaded)
        {
            var mb = MessageBoxManager.GetMessageBoxStandard("No Snapshot", "no snapshot file loaded cant find the difference without it, please load appropriate snapshot file then try again.", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Warning);

            await mb.ShowWindowDialogAsync(WindowHelper.ParentWindow<MainWindow>());
            return;
        }

        DiffWindow window = new DiffWindow();
        await window.ShowDialog(WindowHelper.ParentWindow<MainWindow>()!);
    }

    [RelayCommand]
    public async Task OpenMeasure()
    {
        var eventData = new MeasureTextEvent();
        EventBus.Instance.Publish(eventData);

        if(eventData.Entity == null)
        {
            return;
        }

        MeasureTextWindow window = new MeasureTextWindow(eventData.Entity);
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        await window.ShowDialog(WindowHelper.ParentWindow<MainWindow>()!);
    }

    [RelayCommand]
    public async Task Search()
    {
        FindReplaceWindow fnd = new FindReplaceWindow();
        fnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        await fnd.ShowDialog(WindowHelper.ParentWindow<MainWindow>()!);
    }
}