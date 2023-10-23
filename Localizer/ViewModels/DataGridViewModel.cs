using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Localizer.Core.Helpers;
using Localizer.Core.Resx;
using Microsoft.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        DataGrid!.Columns.Add(CreateColumn(nameof(ResxEntityViewModel.Key)));

        foreach (var culture in ResxManager.Tree.Cultures.OrderBy(x=>x))
        {

            var key = culture == string.Empty ? "ntrKey" : culture;

            key = $"{nameof(ResxEntityViewModel.CultureValues)}[{key}]";

            var languageInfo = Culture.GetLanguageName(culture);
            languageInfo = $"{languageInfo} ({culture})";

            if (culture == string.Empty)
            {
                languageInfo = "Neutral";
            }

            DataGrid.Columns.Add(CreateColumn(languageInfo, key));
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

    private DataGridColumn CreateColumn(string nameOfColumn, string bindingPath=null)
    {
        if (bindingPath == null)
            bindingPath = nameOfColumn;

        // Create the binding
        var binding = new Binding(bindingPath);

        // Create the DataTemplate for normal view
        var normalTemplate = new FuncDataTemplate<ResxEntityViewModel>((value, namescope) =>
            new TextBlock
            {
                [!TextBlock.TextProperty] = binding,
                TextWrapping=TextWrapping.NoWrap,
                TextTrimming=TextTrimming.CharacterEllipsis
            },
            supportsRecycling: true);

        // Create the DataTemplate for editing view
        var editingTemplate = new FuncDataTemplate<ResxEntityViewModel>((value, namescope) =>
            new TextBox 
            {
                [!TextBox.TextProperty] = binding,
                TextWrapping=TextWrapping.WrapWithOverflow,
            }, supportsRecycling: true);

        // Create the column
        var column = new DataGridTemplateColumn
        {
            Header = nameOfColumn,
            CellTemplate = normalTemplate,
            CellEditingTemplate = editingTemplate,
            MaxWidth = 1024,
            Width = new DataGridLength(1,DataGridLengthUnitType.Star),
            MinWidth = 56,
        };

        return column;
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
