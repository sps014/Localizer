using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Localizer.Events;
using Localizer.ViewModels;

namespace Localizer;

public partial class DataGridControl : UserControl
{
    DataGridViewModel viewModel;
    public DataGridControl()
    {
        InitializeComponent();
        viewModel = new DataGridViewModel()
        {
            DataGrid = dataGrid
        };
        DataContext = viewModel;
    }
    public void AddRowNumbers(object sender, DataGridRowEventArgs e)
    {
        e.Row.Header = (e.Row.GetIndex() + 1).ToString();
    }



    private void DataGrid_CellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
    {
        var row = e.Row.DataContext as ResxEntityViewModel;

        viewModel.SaveChanges(row);
    }

    private void DataGrid_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        EventBus.Instance.Publish(new DataGridCurrentSelectionChangedevent(dataGrid.SelectedIndex,
            dataGrid.SelectedItem as ResxEntityViewModel));
    }
}