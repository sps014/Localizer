using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Localizer.ViewModels;

namespace Localizer;

public partial class MeasureTextWindow : Window
{
    MeasureTextViewModel vm;
    public MeasureTextWindow(ResxEntityViewModel entity)
    {
        InitializeComponent();
        DataContext = vm = new MeasureTextViewModel(entity,FontFamily);
        MainWindow.SetThemeOfWindow(this);
        AddColumns();
    }

    void AddColumns()
    {
        AddColumn(nameof(MesaureResultItem.Culture));
        AddColumn(nameof(MesaureResultItem.Text));
        AddColumn(nameof(MesaureResultItem.Length));

    }

    void AddColumn(string path)
    {

        dataGrid.Columns.Add(new DataGridTextColumn()
        {
            Header = path,
            Binding = new Binding(path),
        });
    }
}