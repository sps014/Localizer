using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Data;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Localizer.Core.Resx;

namespace Localizer.ViewModels;

internal partial class DataGridViewModel:ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ResxEntityViewModel> entries=new();

    private ResxManager ResxManager;

    [ObservableProperty]
    public ObservableCollection<ResxEntityViewModel> source;

    public required DataGrid? DataGrid { get; set; }

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
       

        DataGrid.Columns.Add(new DataGridTextColumn()
        {
            Header = nameof(ResxEntityViewModel.Key),
            Binding = new Binding(nameof(ResxEntityViewModel.Key),BindingMode.TwoWay),
        });

        foreach(var culture in ResxManager.Tree.Cultures)
        {
            var key = culture == string.Empty ? "ntrKey" : culture;

            DataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = culture==string.Empty?"Neutral":culture,
                Binding = new Binding(nameof(ResxEntityViewModel.CultureValues) + $"[{key}]", BindingMode.TwoWay),
            });
        }

        var entries = ResxManager.ResxEntities;
        foreach (var entry in entries)
        {
            Entries.Add(new ResxEntityViewModel(entry)
            {
                Key = entry.Key,
            });
        }

        Source = Entries;
    }

}

public class ResxEntityViewModel
{
    public string? Key { get; set; }
    public Dictionary<string, string?> CultureValues { get; set; } = new();

    public ResxEntityViewModel(ResxEntity entity)
    {
        foreach(var culture in MainWindowViewModel.Instance!.ResxManager.Tree.Cultures)
        {
            if(culture==string.Empty)
                CultureValues.Add("ntrKey", entity.GetValue(culture));
            else
            CultureValues.Add(culture,entity.GetValue(culture));
        }
    }
}
