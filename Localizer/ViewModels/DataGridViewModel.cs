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
using Localizer.Events;

namespace Localizer.ViewModels;

internal partial class DataGridViewModel:ObservableObject
{

    private ResxManager ResxManager;

    [ObservableProperty]
    public ObservableCollection<ResxEntityViewModel> source=new();

    public required DataGrid? DataGrid { get; set; }

    public DataGridViewModel()
    {
        ResxManager = MainWindowViewModel.Instance!.ResxManager;
        ResxManager.OnResxReadFinished += ResxManager_OnResxReadFinished;
        EventBus.Instance.Subscribe<TableColmnVisibilityChangeEvent>(OnLanguageColumnVisiblityChanged);
    }

    public void OnLanguageColumnVisiblityChanged(TableColmnVisibilityChangeEvent e)
    {
        var nameOfTheColumn = GetColumnHeaderName(e.ColumnInfo.Culture, e.ColumnInfo.IsComment);
        var column = DataGrid!.Columns.FirstOrDefault(x => nameOfTheColumn.Equals(x.Tag));
        
        if(column==null)
            return;

        column.IsVisible = e.ColumnInfo.IsVisible;
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

            //for value col
            DataGrid.Columns.Add(CreateColumn(GetColumnHeaderName(culture), GetKeyPath(culture)));

            //for comment col
            DataGrid.Columns.Add(CreateColumn(GetColumnHeaderName(culture,true), GetKeyPath(culture,true)));

        }

        var entries = ResxManager.ResxEntities;
        foreach (var entry in entries)
        {
            Source.Add(new ResxEntityViewModel(entry)
            {
                Key = entry.Key,
            });
        }
    }

    string GetKeyPath(string culture, bool isComment=false)
    {

        var key = culture == string.Empty ? "ntrKey" : culture;


        if(isComment)
            key = $"{nameof(ResxEntityViewModel.CultureComments)}[{key}]";
        else
            key = $"{nameof(ResxEntityViewModel.CultureValues)}[{key}]";

        return key ;
    }
    string GetColumnHeaderName(string culture,bool isComment=false)
    {
        if(isComment)
        {
            var language = $"Comment ({culture})";

            if (culture == string.Empty)
            {
                language = "Comment Neutral";
            }
            return language;
        }

        var languageInfo = Culture.GetLanguageName(culture);
        languageInfo = $"{languageInfo} ({culture})";

        if (culture == string.Empty)
        {
            languageInfo = "Neutral";
        }

        return languageInfo;
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
                TextTrimming=TextTrimming.CharacterEllipsis,
                VerticalAlignment=Avalonia.Layout.VerticalAlignment.Center,
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
            Tag = nameOfColumn,
        };

        return column;
    }

    internal void SaveChanges(ResxEntityViewModel? row)
    {
        throw new NotImplementedException();
    }
}



public class ResxEntityViewModel
{
    public string? Key { get; set; }
    public Dictionary<string, string?> CultureValues { get; set; } = new();
    public Dictionary<string, string?> CultureComments { get; set; } = new();
    public ResxEntity ResxEntity { get; init; }

    public ResxEntityViewModel(ResxEntity entity)
    {
        ResxEntity = entity;
        foreach(var culture in MainWindowViewModel.Instance!.ResxManager.Tree.Cultures)
        {
            if (culture == string.Empty)
            {
                CultureValues.Add("ntrKey", entity.GetValue(culture));
                CultureComments.Add("ntrKey", entity.GetComment(culture));
            }
            else
            {
                CultureValues.Add(culture, entity.GetValue(culture));
                CultureComments.Add(culture, entity.GetComment(culture));
            }

        }
    }
}
