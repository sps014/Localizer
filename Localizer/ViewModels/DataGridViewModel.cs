using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using Localizer.Core.Resx;

namespace Localizer.ViewModels;

internal partial class DataGridViewModel:ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ResxEntityViewModel> entries=new();

    private ResxManager ResxManager;

    [ObservableProperty]
    public FlatTreeDataGridSource<ResxEntityViewModel> source;

    public DataGridViewModel()
    {
        ResxManager = MainWindowViewModel.Instance!.ResxManager;
        ResxManager.OnResxReadFinished += ResxManager_OnResxReadFinished;
    }

    private void ResxManager_OnResxReadFinished(object sender, Core.Model.ResxReadFinishedEventArgs e)
    {
        LoadEntries();
    }
    void LoadEntries()
    {
        var entries = ResxManager.ResxEntities;
        foreach (var entry in entries)
        {
            Entries.Add(new ResxEntityViewModel(entry)
            {
                Key = entry.Key,
            });
        }

        Source = new FlatTreeDataGridSource<ResxEntityViewModel>(Entries);

        Source.Columns.Add(new TextColumn<ResxEntityViewModel, string>(nameof(ResxEntityViewModel.Key), x => x.Key));

        foreach(var c in ResxManager.Tree.Cultures)
        {
            var header = c == string.Empty ? "Neutral" : c;
            Source.Columns.Add(new TextColumn<ResxEntityViewModel, string>(header, x => x.CultureValues[c]));
        }
    }

}

public class ResxEntityViewModel
{
    public string? Key { get; set; }
    public Dictionary<string, string?> CultureValues { get; set; } = new();

    public ResxEntityViewModel(ResxEntity entity)
    {
        foreach(var culture in entity.Cultures)
        {
            CultureValues.Add(culture,entity.GetValue(culture));
        }
    }
}
